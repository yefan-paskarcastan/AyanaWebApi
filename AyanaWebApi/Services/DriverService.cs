using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Microsoft.EntityFrameworkCore;

using MihaZupan;
using HtmlAgilityPack;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Services
{
    public class DriverService : IDriverService
    {
        public DriverService(AyDbContext ayDbContext,
                             IImghostService imghostService,
                             IImgsConverterService imgsConverterService)
        {
            _context = ayDbContext;
            _imghostService = imghostService;
            _imgsConverter = imgsConverterService;
        }

        /// <summary>
        /// Подготавливает распрашеный пост к выкладыванию и сохраняет его в базу
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<TorrentSoftPost> RutorTorrent(DriverRutorTorrentInput param)
        {
            TorrentSoftPost post = await RutorTorrentTest(param);
            if (post != null)
            {
                _context.TorrentSoftPosts.Add(post);
                _context.SaveChanges();
                foreach (var item in post.Screenshots)
                {
                    item.TorrentSoftPost = null;
                }
                return post;
            }
            return null;
        }

        /// <summary>
        /// Подготавливает распрашеный пост к выкладыванию
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<TorrentSoftPost> RutorTorrentTest(DriverRutorTorrentInput param)
        {
            RutorItem rutorItem = _context
                                  .RutorItems
                                  .Include(el => el.RutorListItem)
                                  .Include(el => el.Imgs)
                                  .Include(el => el.Spoilers)
                                  .SingleOrDefault(el => el.Id == param.ParseItemId);

            if (rutorItem != null)
            {
                var post = new TorrentSoftPost();
                post.Name = rutorItem.Name;

                string torrentFile = await DownloadFile(param.TorrentUri + rutorItem.RutorListItem.HrefNumber,
                                                        Path.GetRandomFileName().Replace('.', '_') + ".torrent",
                                                        param.ProxySocks5Addr,
                                                        param.ProxySocks5Port);
                if (torrentFile == null)
                {
                    _context.Logs.Add(new Log
                    {
                        Created = DateTime.Now,
                        Location = "DriverService / Rutor Torrent / Преобразование раздачи",
                        Message = $"Не удалось загрузить торрент файл. RutorItemId: {param.ParseItemId}; Href: {rutorItem.RutorListItem.HrefNumber}",
                    });
                    _context.SaveChanges();
                    return null;
                }
                post.TorrentFile = torrentFile;

                string posterFile = await GetPosterImg(rutorItem.Imgs, param);
                if (posterFile == null)
                {
                    _context.Logs.Add(new Log
                    {
                        Created = DateTime.Now,
                        Location = "DriverService / Rutor Torrent / Преобразование раздачи",
                        Message = $"Не удалось загрузить постер. RutorItemId: {param.ParseItemId}; Href: {rutorItem.RutorListItem.HrefNumber}",
                    });
                    _context.SaveChanges();
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
                     select new TorrentSoftPostScreenshot
                     {
                         Created = DateTime.Now,
                         ScreenUri = el,
                     }).ToList();
                post.Screenshots = queryScr;

                post.Description = FormatDescription(rutorItem.Description, post.PosterImg);
                post.Spoilers = FormatSpoilers(rutorItem.Spoilers);

                return post;
            }
            _context.Logs.Add(new Log
            {
                Created = DateTime.Now,
                Location = "DriverService / Rutor Torrent / Преобразование раздачи",
                Message = "В базе не найдена раздача с указанным Id",
            });
            _context.SaveChanges();
            return null;
        }

        #region Private
        readonly AyDbContext _context;

        readonly IImghostService _imghostService;

        readonly IImgsConverterService _imgsConverter;

        /// <summary>
        /// Загружает файл в локальную папку
        /// </summary>
        /// <param name="uri">Расположение файла</param>
        /// <param name="fileName">Не полное имя файла с расширением</param>
        /// <param name="proxyAddress">Адрес тор прокси</param>
        /// <param name="proxyPort">Порт тор проски</param>
        /// <returns></returns>
        async Task<string> DownloadFile(string uri, 
                                        string fileName, 
                                        string proxyAddress, 
                                        int proxyPort)
        {
            var webClient = new WebClient();
            webClient.Proxy = new HttpToSocks5Proxy(proxyAddress, proxyPort);

            string folderName = Environment.CurrentDirectory 
                                    + "\\storage\\"
                                    + DateTime.Today.ToString("yyyy.MM.dd");
            Directory.CreateDirectory(folderName);
            string fullName = folderName + "\\" + GetSafeFilename(fileName);

            try
            {
                await webClient.DownloadFileTaskAsync(uri, fullName);
            }
            catch (WebException ex)
            {
                _context.Logs.Add(new Log
                {
                    Created = DateTime.Now,
                    Location = "Driver Service / Download File / Загрузка файла",
                    Message = "При загрузке произошла ошибка. Указан неврный адрес или произошла другая сетевая ошибка",
                    StackTrace = ex.StackTrace
                });
                _context.SaveChanges();
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
        async Task<string> GetPosterImg(List<RutorItemImg> listImg, 
                                        DriverRutorTorrentInput param)
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
                                                                param.ProxySocks5Port);
                    if (fullFileNameOne == null)
                    {
                        _context.Logs.Add(new Log
                        {
                            Created = DateTime.Now,
                            Location = "Driver Service / Get Poster Img / Загрузка постера",
                            Message = "При загрузке постера произошла ошибка",
                        });
                        _context.SaveChanges();
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
                        _context.Logs.Add(new Log
                        {
                            Created = DateTime.Now,
                            Location = "Driver Service / Get Poster Img / Выбор постера",
                            Message = $"При загрузке файла постера произошла ошибка. Url {item.ChildUrl}",
                            StackTrace = ex.StackTrace,
                        });
                        _context.SaveChanges();
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
                                                         param.ProxySocks5Port);
                if (fullFileName == null)
                {
                    _context.Logs.Add(new Log
                    {
                        Created = DateTime.Now,
                        Location = "Driver Service / Get Poster Img / Загрузка постера",
                        Message = "При загрузке постера произошла ошибка",
                    });
                    _context.SaveChanges();
                    return null;
                }
                return fullFileName;
            }
            _context.Logs.Add(new Log
            {
                Created = DateTime.Now,
                Location = "Driver Service / Get Poster Img / Загрузка постера",
                Message = "Не удалось выбрать подходящий постер или изображения отсутсвуют",
            });
            _context.SaveChanges();
            return null;
        }

        /// <summary>
        /// Подготавливает описание
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        string FormatDescription(string desc, string poster)
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
                item.Remove();
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
        string FormatSpoilers(List<RutorItemSpoiler> lst)
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
