using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;

using MihaZupan;
using AyanaWebApi.Models;
using System.Drawing;

namespace AyanaWebApi.Services
{
    public class DriverService : IDriverService
    {
        public DriverService(AyDbContext ayDbContext)
        {
            _context = ayDbContext;
        }

        /// <summary>
        /// Подготавливает распрашеный пост к выкладыванию
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<string> RutorTorrent(DriverRutorTorrentInput param)
        {
            RutorItem rutorItem = _context
                                  .RutorItems
                                  .Include(el => el.RutorListItem)
                                  .Include(el => el.Imgs)
                                  .SingleOrDefault(el => el.Id == param.ParseItemId);

            if (rutorItem != null)
            {
                var post = new TorrentSoftPost();
                post.Name = rutorItem.Name;
                string torrentFile = await DownloadFile(param.TorrentUri + rutorItem.RutorListItem.HrefNumber,
                                                        rutorItem.Name + ".torrent",
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
                post.PosterImg = posterFile;

                return posterFile;
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
                    Stream stream = await webClient.OpenReadTaskAsync(item.ChildUrl);
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
        #endregion
    }
}
