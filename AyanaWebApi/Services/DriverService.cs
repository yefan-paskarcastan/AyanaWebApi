using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Microsoft.EntityFrameworkCore;

using MihaZupan;
using HtmlAgilityPack;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services
{
    public class DriverService : IDriverService
    {
        public DriverService(AyDbContext ayDbContext,
                             IImghostService imghostService,
                             IImgsConverterService imgsConverterService,
                             ILogService logService)
        {
            _context = ayDbContext;
            _imghostService = imghostService;
            _imgsConverter = imgsConverterService;
            _logService = logService;
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
        readonly AyDbContext _context;
        readonly IImghostService _imghostService;
        readonly IImgsConverterService _imgsConverter;
        readonly ILogService _logService;

        /// <summary>
        /// Преобразует RutorPost в SoftPost
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<SoftPost> RutorToSoft(DriverToSoftInput param)
        {
            var serviceResult = new ServiceResult<string>
            {
                ServiceName = nameof(DriverService),
                Location = "Преобразование презентации в пост на сайт soft"
            };

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
                    serviceResult.Comment = "Не удалось загрузить торрент файл";
                    serviceResult.ErrorContent = $"RutorItemId: {param.ParseItemId}; Href: {rutorItem.RutorListItem.HrefNumber}";
                    _logService.Write(serviceResult);
                    return null;
                }
                post.TorrentFile = torrentFile;

                string posterFile = await GetPosterImgRutor(rutorItem.Imgs, param);
                if (posterFile == null)
                {
                    serviceResult.Comment = "Не удалось загрузить постер";
                    serviceResult.ErrorContent = $"RutorItemId: {param.ParseItemId}; Href: {rutorItem.RutorListItem.HrefNumber}";
                    _logService.Write(serviceResult);
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
            serviceResult.Comment = "В базе не найдена раздача с указанным Id";
            serviceResult.ErrorContent = $"RutorItemId: {param.ParseItemId}";
            _logService.Write(serviceResult);
            return null;
        }

        /// <summary>
        /// Преобразует NnmclubPost в SoftPost
        /// </summary>
        /// <returns></returns>
        async Task<SoftPost> NnmclubToSoft(DriverToSoftInput param)
        {
            var serviceResult = new ServiceResult<string>
            {
                ServiceName = nameof(DriverService),
                Location = "Преобразование презентации в пост на сайт soft"
            };

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
                    serviceResult.Comment = "Не удалось загрузить постер";
                    serviceResult.ErrorContent = $"NnmclubItemId: {param.ParseItemId}; Href: {clubItem.NnmclubListItem.Href}";
                    _logService.Write(serviceResult);
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
                    serviceResult.Comment = "Не удалось загрузить торрент файл";
                    serviceResult.ErrorContent = $"NnmclubItemId: {param.ParseItemId}; Href: {clubItem.NnmclubListItem.Href}";
                    _logService.Write(serviceResult);
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
            serviceResult.Comment = "В базе не найдена презентация с указанным Id";
            serviceResult.ErrorContent = $"NnmclubItemId: {param.ParseItemId}";
            _logService.Write(serviceResult);
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
            var serviceResult = new ServiceResult<string>
            {
                ServiceName = nameof(DriverService),
                Location = "Загрузка файла"
            };

            var webClient = new WebClient();
            if(proxyUsing)
                webClient.Proxy = new HttpToSocks5Proxy(proxyAddress, proxyPort);
            if (authPage != null)
            {
                webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                await webClient.UploadStringTaskAsync(authPage, authParam);
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
                serviceResult.Comment = "Указан неврный адрес или произошла другая сетевая ошибка";
                serviceResult.ErrorContent = "Uri: " + uri;
                serviceResult.ExceptionMessage = ex.Message;
                _logService.Write(serviceResult);
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
            var serviceResult = new ServiceResult<string>
            {
                ServiceName = nameof(DriverService),
                Location = "Выбор постера"
            };

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
                        serviceResult.Comment = "При загрузке постера произошла ошибка";
                        serviceResult.ErrorContent = "ChildImg uri: " + img.ChildUrl;
                        _logService.Write(serviceResult);
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
                        serviceResult.Comment = "При загрузке файла постера по списку произошла ошибка";
                        serviceResult.ErrorContent = "ChildImg uri: " + item.ChildUrl;
                        serviceResult.ExceptionMessage = ex.Message;
                        _logService.Write(serviceResult);
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
                    serviceResult.Comment = "При загрузке постера произошла ошибка";
                    serviceResult.ErrorContent = "Poster uri: " + posterUri;
                    _logService.Write(serviceResult);
                    return null;
                }
                return fullFileName;
            }
            serviceResult.Comment = "Не удалось выбрать подходящий постер или изображения отсутсвуют";
            _logService.Write(serviceResult);
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
