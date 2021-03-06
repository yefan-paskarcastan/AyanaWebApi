﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MihaZupan;
using HtmlAgilityPack;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Services
{
    public class DriverService : IDriverService
    {
        readonly AyDbContext _context;
        readonly ILogger<DriverService> _logger;
        readonly IImghostService _imghostService;
        readonly IImgsConverterService _imgsConverter;

        public DriverService(AyDbContext ayDbContext,
                             IImghostService imghostService,
                             IImgsConverterService imgsConverterService,
                             ILogger<DriverService> logger)
        {
            _context = ayDbContext;
            _imghostService = imghostService;
            _imgsConverter = imgsConverterService;
            _logger = logger;
        }

        /// <summary>
        /// Преобразует пост в SoftPost
        /// </summary>
        /// <param name="param"></param>
        /// <returns>SoftPost</returns>
        public async Task<SoftPost> Convert(DriverToSoftInput param)
        {
            SoftPost post = null;
            switch (param.Type)
            {
                case nameof(RutorItem):
                    post = await RutorToSoft(param);
                    break;
                case nameof(NnmclubItem):
                    post = await NnmclubToSoft(param);
                    break;
                default:
                    break;
            }
            if (post != null)
            {
                _context.SoftPosts.Add(post);
                _context.SaveChanges();
                return post;
            }
            return null;
        }

        #region Private
        /// <summary>
        /// Преобразует RutorPost в SoftPost
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<SoftPost> RutorToSoft(DriverToSoftInput param)
        {
            RutorItem rutorItem = _context
                                  .RutorItems
                                  .Include(el => el.RutorListItem)
                                  .Include(el => el.Imgs)
                                  .Include(el => el.Spoilers)
                                  .SingleOrDefault(el => el.Id == param.ParseItemId);

            if (rutorItem != null)
            {
                var post = new SoftPost();
                post.Name = rutorItem.Name;
                post.Created = DateTime.Now;

                string torrentFile = await DownloadFile(param.TorrentUri + rutorItem.RutorListItem.HrefNumber,
                                                        Path.GetRandomFileName().Replace('.', '_') + ".torrent",
                                                        param.ProxySocks5Addr,
                                                        param.ProxySocks5Port,
                                                        param.ProxyActive);
                if (torrentFile == null)
                {
                    _logger.LogError($"Не удалось загрузить торрент файл. RutorItem.Id: {param.ParseItemId}; Href: {rutorItem.RutorListItem.HrefNumber}");
                    return null;
                }
                post.TorrentFile = torrentFile;

                string posterFile = await GetPosterImgRutor(rutorItem.Imgs, param);
                if (posterFile == null)
                {
                    _logger.LogError($"Не удалось загрузить постер. RutorItem.Id: {param.ParseItemId}; Href: {rutorItem.RutorListItem.HrefNumber}");
                    return null;
                }
                FileInfo img = new FileInfo(posterFile);
                if (img.Length > param.MaxPosterSize * 1024)
                {
                    posterFile = _imgsConverter.ConvertToJpg(posterFile, 100);
                }
                post.PosterImg = posterFile;

                var queryUriImgs =
                    (from el in rutorItem.Imgs
                     where el.ParentUrl != null
                     select el.ParentUrl).ToList();
                var queryParams =
                    (from el in _context.ImghostParsingInputs
                     where el.Active == true
                     select el).ToList();
                List<string> screenshots = 
                    (await _imghostService.GetOriginalsUri(new ImghostGetOriginalsInput
                    {
                        ImgsUri = queryUriImgs,
                        ParsingParams = queryParams,
                    })).ToList();
                var queryScr =
                    (from el in screenshots
                     select new SoftPostImg
                     {
                         Created = DateTime.Now,
                         ImgUri = el,
                     }).ToList();
                post.Imgs = queryScr;

                post.Description = FormatDescriptionRutor(rutorItem.Description, post.PosterImg);
                post.Spoilers = FormatSpoilersRutor(rutorItem.Spoilers);

                return post;
            }
            _logger.LogError($"В базе не найдена раздача с указанным Id. RutorItem.Id: {param.ParseItemId}");
            return null;
        }

        /// <summary>
        /// Преобразует NnmclubPost в SoftPost
        /// </summary>
        /// <returns></returns>
        async Task<SoftPost> NnmclubToSoft(DriverToSoftInput param)
        {
            NnmclubItem clubItem = _context
                                  .NnmclubItems
                                  .Include(el => el.NnmclubListItem)
                                  .Include(el => el.Imgs)
                                  .Include(el => el.Spoilers)
                                  .SingleOrDefault(el => el.Id == param.ParseItemId);
            if (clubItem != null)
            {
                string posterFullName = await DownloadFile(clubItem.Poster,
                                                           Path.GetFileName(clubItem.Poster),
                                                           param.ProxySocks5Addr,
                                                           param.ProxySocks5Port,
                                                           param.ProxyActive);
                if (posterFullName == null)
                {
                    _logger.LogError($"Не удалось загрузить постер. NnmclubItem.Id: {param.ParseItemId}; Href: {clubItem.NnmclubListItem.Href}");
                    return null;
                }

                string torrentFullName = await DownloadFile(param.TorrentUri + clubItem.Torrent,
                                                            Path.GetRandomFileName().Replace('.', '_') + ".torrent",
                                                            param.ProxySocks5Addr,
                                                            param.ProxySocks5Port,
                                                            param.ProxyActive,
                                                            new Uri(param.AuthPage),
                                                            param.AuthParam);
                if (torrentFullName == null)
                {
                    _logger.LogError($"Не удалось загрузить торрент файл. NnmclubItem.Id: {param.ParseItemId}; Href: {clubItem.NnmclubListItem.Href}");
                    return null;
                }

                var post = new SoftPost
                {
                    Created = DateTime.Now,
                    Name = clubItem.Name,
                    Description = clubItem.Description,
                    Spoilers = FormatSpoilersNnmclub(clubItem.Spoilers),
                    PosterImg = posterFullName,
                    TorrentFile = torrentFullName,
                    Imgs = (from img in clubItem.Imgs
                            select new SoftPostImg
                            {
                                Created = DateTime.Now,
                                ImgUri = img.ImgUri
                            }).ToList(),
                };
                return post;
            }
            _logger.LogError($"В базе не найдена презентация с указанным Id. NnmclubItem.Id: {param.ParseItemId}");
            return null;
        }

        /// <summary>
        /// Загружает файл в локальную папку
        /// </summary>
        /// <param name="uri">Расположение файла</param>
        /// <param name="fileName">Не полное имя файла с расширением</param>
        /// <param name="proxyAddress">Адрес тор прокси</param>
        /// <param name="proxyPort">Порт тор проски</param>
        /// <param name="proxyUsing">Использовать или нет тор прокси</param>
        /// <param name="authPage">Адрес страницы для авторизации, если требуется</param>
        /// <param name="authParam">Параметры запроса для авторизации</param>
        /// <returns>Полное имя сохранненого файла</returns>
        async Task<string> DownloadFile(string uri, 
                                        string fileName, 
                                        string proxyAddress, 
                                        int proxyPort,
                                        bool proxyUsing,
                                        Uri authPage = null,
                                        string authParam = "")
        {
            var webClient = new WebClient();
            if(proxyUsing)
                webClient.Proxy = new HttpToSocks5Proxy(proxyAddress, proxyPort);
            if (authPage != null)
            {
                //todo : иногда не работает авторизация
                /*webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                await webClient.UploadStringTaskAsync(authPage, authParam); */
                webClient.Headers.Add(HttpRequestHeader.Cookie, "phpbb2mysql_4_data=a%3A2%3A%7Bs%3A11%3A%22autologinid%22%3Bs%3A32%3A%22cb75a18e5540db38390daca79665397f%22%3Bs%3A6%3A%22userid%22%3Bi%3A10507528%3B%7D; phpbb2mysql_4_sid=f48e60f3de704a038c4e85a3285d1aee; phpbb2mysql_4_t=a%3A1%3A%7Bi%3A1443472%3Bi%3A1611880370%3B%7D");
            }

            string folderName = Environment.CurrentDirectory 
                                    + "\\storage\\"
                                    + DateTime.Today.ToString("yyyy.MM.dd");
            Directory.CreateDirectory(folderName);
            string fullName = folderName + "\\" + GetSafeFilename(fileName);
            if (File.Exists(fullName)) return fullName;

            try
            {
                await webClient.DownloadFileTaskAsync(uri, fullName);
            }
            catch (WebException ex)
            {
                _logger.LogError(ex, "Указан неврный адрес или произошла другая сетевая ошибка. Uri: " + uri);
                return null;
            }
            return fullName;
        }

        /// <summary>
        /// Возвращает строку, котора может быть использована для имени файла в Windows
        /// </summary>
        /// <param name="filename">Строка в которой могут содержаться недопустимые символы в имени файла</param>
        /// <returns></returns>
        string GetSafeFilename(string filename)
        {
            string[] stringArr = filename.Split(Path.GetInvalidFileNameChars());
            return string.Join("", stringArr);
        }

        /// <summary>
        /// Выбирает постер из списка изображений и сохраняет его
        /// </summary>
        /// <param name="listImg"></param>
        /// <returns>Полное имя файла</returns>
        async Task<string> GetPosterImgRutor(List<RutorItemImg> listImg, 
                                             DriverToSoftInput param)
        {
            IList<RutorItemImg> withoutLink =
                (from img in listImg
                 where img.ParentUrl == null && img.ChildUrl != null
                 select img).ToList();

            if (withoutLink.Count > 0)
            {
                if (withoutLink.Count == 1)
                {
                    RutorItemImg img = withoutLink.Single();
                    string fullFileNameOne = await DownloadFile(img.ChildUrl,
                                                                Path.GetFileName(img.ChildUrl),
                                                                param.ProxySocks5Addr,
                                                                param.ProxySocks5Port,
                                                                param.ProxyActive);
                    if (fullFileNameOne == null)
                    {
                        _logger.LogError("При загрузке постера произошла ошибка. ChildImg uri: " + img.ChildUrl);
                        return null;
                    }
                    return fullFileNameOne;
                }

                int maxSize = 0;
                string posterUri = "";
                foreach (RutorItemImg item in withoutLink)
                {
                    var webClient = new WebClient();
                    Stream stream;
                    try
                    {
                        stream = await webClient.OpenReadTaskAsync(item.ChildUrl);
                    }
                    catch (WebException ex)
                    {
                        _logger.LogError(ex, "При загрузке файла постера по списку произошла ошибка. ChildImg uri: " + item.ChildUrl);
                        return null;
                    }
                    var img = new Bitmap(stream);
                    if (maxSize < img.Width * img.Height)
                    {
                        maxSize = img.Width * img.Height;
                        posterUri = item.ChildUrl;
                    }
                }

                string fullFileName = await DownloadFile(posterUri,
                                                         Path.GetFileName(posterUri),
                                                         param.ProxySocks5Addr,
                                                         param.ProxySocks5Port,
                                                         param.ProxyActive);
                if (fullFileName == null)
                {
                    _logger.LogError("При загрузке постера произошла ошибка. Poster uri: " + posterUri);
                    return null;
                }
                return fullFileName;
            }
            _logger.LogError("Не удалось выбрать подходящий постер или изображения отсутсвуют");
            return null;
        }

        /// <summary>
        /// Подготавливает описание
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        string FormatDescriptionRutor(string desc, string poster)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(desc);

            HtmlNode htmlNode = htmlDocument.DocumentNode;
            HtmlNodeCollection nodesScrenshots = htmlNode.SelectNodes(@"//img[parent::a]");

            if (nodesScrenshots != null)
            {
                foreach (var item in nodesScrenshots)
                {
                    item.Remove();
                }
            }

            HtmlNodeCollection nodesImgs = htmlNode.SelectNodes(@"//img");
            if (nodesImgs != null && nodesImgs.Count == 2)
            {
                var item = nodesImgs.Where(el => el.GetAttributeValue("src", null)
                                                    .Contains(Path.GetFileName(poster)))
                                                    .SingleOrDefault();
                item?.Remove();
            }
            else
            {
                foreach (var item in nodesImgs)
                {
                    item.Remove();
                }
            }

            string description = htmlNode.OuterHtml.Replace("<div></div>", "");

            description = description.Replace("<hr>", "");
            description = description.Replace("<br>", "");

            while (description.Contains(Environment.NewLine + Environment.NewLine))
            {
                description = description.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            }
            return description;
        }

        /// <summary>
        /// Подготавливает сполйеры
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        string FormatSpoilersRutor(List<RutorItemSpoiler> lst)
        {
            string spoilersResult = Environment.NewLine + Environment.NewLine;

            foreach (var item in lst)
            {
                spoilersResult += $"[spoiler={item.Header}]{item.Body}[/spoiler]";
            }
            return spoilersResult;
        }

        string FormatSpoilersNnmclub(List<NnmclubItemSpoiler> lst)
        {
            string spoilersResult = Environment.NewLine + Environment.NewLine;

            foreach (var item in lst)
            {
                spoilersResult += $"[spoiler={item.Header}]{item.Body}[/spoiler]";
            }
            return spoilersResult;
        }
        #endregion
    }
}
