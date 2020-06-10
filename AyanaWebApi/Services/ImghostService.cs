using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Services
{
    public class ImghostService : IImghostService
    {
        public ImghostService(AyDbContext ayDbContext)
        {
            _context = ayDbContext;
        }

        /// <summary>
        /// Возвращает прямые ссылки для указанных изображений
        /// </summary>
        /// <param name="listImg"></param>
        /// <returns></returns>
        public async Task<IList<string>> GetOriginalsUri(ImghostGetOriginalsInput param)
        {
            var listImg = new List<string>();
            foreach (string item in param.ImgsUri)
            {
                foreach (ImghostParsingInput parsParam in param.ParsingParams)
                {
                    if (item.Contains(parsParam.Def))
                    {
                        listImg.Add(await GetOriginalUriImg(item, parsParam.XPath, parsParam.Attr));
                    }
                }
            }
            return listImg;
        }

        /// <summary>
        /// Возвращает указанную веб страницу
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        async Task<string> GetPage(string uri)
        {
            var webClient = new WebClient();

            byte[] data = null;
            try
            {
                data = await webClient.DownloadDataTaskAsync(new Uri(uri));
            }
            catch (WebException ex)
            {
                _context.Logs.Add(new Log
                {
                    Created = DateTime.Now,
                    Location = "Imghost Service / Get Page / Загрузка веб страницы",
                    Message = "При загрузке произошла ошибка. Указан неврный адрес или произошла другая сетевая ошибка",
                    StackTrace = ex.StackTrace
                });
                _context.SaveChanges();
                return null;
            }

            if (data != null) return Encoding.UTF8.GetString(data);
            return null;
        }

        /// <summary>
        /// Возвращает прямую ссылку на изображение
        /// </summary>
        /// <param name="uri">Путь до картинки</param>
        /// <param name="xPath">Выражение XPath для извлечения изображения</param>
        /// <returns></returns>
        async Task<string> GetOriginalUriImg(string uri, string xPath, string htmlAttr)
        {
            string page = await GetPage(uri);
            if (page != null)
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(page);
                HtmlNodeCollection docNode = htmlDocument.DocumentNode.SelectNodes(xPath);
                if (docNode != null && docNode.Count == 1)
                {
                    string fullUriImg = docNode[0].GetAttributeValue(htmlAttr, null);
                    string[] withoutQueryString = fullUriImg.Split('?');
                    return withoutQueryString[0];
                }
                _context.Logs.Add(new Log
                {
                    Created = DateTime.Now,
                    Location = "Imghost Service / Standart Img Handler / Получение прямой ссылки на изображение",
                    Message = $"Не удалось однозначно определить изображение на странице. Неверное XPath выражение. Uri: {uri}",
                });
                _context.SaveChanges();
                return uri;
            }
            _context.Logs.Add(new Log
            {
                Created = DateTime.Now,
                Location = "Imghost Service / Standart Img Handler / Получение прямой ссылки на изображение",
                Message = "При загрузке произошла ошибка. Указан неврный адрес или произошла другая сетевая ошибка",
            });
            _context.SaveChanges();
            return uri;
        }

        readonly AyDbContext _context;
    }
}
