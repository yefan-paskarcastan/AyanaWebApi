using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services
{
    public class ImghostService : IImghostService
    {
        public ImghostService(ILogService logService)
        {
            _logService = logService;
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
                var serviceResult = new ServiceResult<string>
                {
                    ServiceName = "ImghostService",
                    Location = "Загрузка веб страницы",
                    Comment = "Указан неврный адрес или произошла другая сетевая ошибка",
                    ExceptionMessage = ex.Message
                };
                _logService.Write(serviceResult);
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
            var serviceResult = new ServiceResult<string>
            {
                ServiceName = "ImghostService",
                Location = "Выпрямитель ссылок с хостинга картинок"
            };

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
                serviceResult.Comment = "Неверное XPath выражение";
                serviceResult.ErrorContent = $"Uri: {uri}";
                _logService.Write(serviceResult);
                return uri;
            }
            serviceResult.Comment = "Указан неврный адрес или произошла другая сетевая ошибка";
            serviceResult.ErrorContent = $"Uri: {uri}";
            _logService.Write(serviceResult);
            return uri;
        }

        readonly ILogService _logService;
    }
}
