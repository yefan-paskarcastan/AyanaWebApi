using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Globalization;
using System.Web;

using Microsoft.Extensions.Logging;

using HtmlAgilityPack;
using MihaZupan;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Services
{
    /// <summary>
    /// Парсит презентации с клуба
    /// </summary>
    public class NnmclubService : INnmclubService
    {
        readonly AyDbContext _context;
        readonly ILogger<NnmclubService> _logger;

        public NnmclubService(AyDbContext ayDbContext,
                              ILogger<NnmclubService> logger)
        {
            _context = ayDbContext;
            _logger = logger;
        }

        /// <summary>
        /// Парсит указанную раздачу и записывает ее в базу
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<NnmclubItem> ParseItem(NnmclubParseItemInput param)
        {
            NnmclubItem item = await GetItem(param);
            if (item != null)
            {
                _context.NnmclubItems.Add(item);
                _context.SaveChanges();
                return item;
            }
            _logger.LogError("Не удалось распрарсить презентацию");
            return null;
        }

        /// <summary>
        /// Проверить список презентаций клуба
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IList<NnmclubListItem>> CheckList(NnmclubCheckListInput param)
        {
            IList<NnmclubListItem> items = await GetList(param);

            if (items != null)
            {
                if (_context.NnmclubListItems.Count() == 0)
                {
                    _context.NnmclubListItems.AddRange(items);
                    _context.SaveChanges();
                    return items;
                }
                IList<NnmclubListItem> oldItems =
                    _context
                    .NnmclubListItems
                    .OrderByDescending(d => d.Created)
                    .Take(200)
                    .ToList();
                IList<NnmclubListItem> onlyNew =
                    items.Except(oldItems, new NnmclubListItemComparer())
                    .ToList();

                _context.NnmclubListItems.AddRange(onlyNew);
                _context.SaveChanges();
                return onlyNew;
            }
            _logger.LogError("При получении последних презентаций вернулось null");
            return null;
        }

        #region Private
        /// <summary>
        /// Получает последние презентации клуба
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<IList<NnmclubListItem>> GetList(NnmclubCheckListInput param)
        {
            var result = new List<NnmclubListItem>();

            for (int i = param.UriListCount; i >= 0; i--)
            {
                string page = await GetPage(
                    param.UriList + (i * param.UriListIncrement),
                    param.ProxySocks5Addr,
                    param.ProxySocks5Port,
                    param.ProxyActive,
                    new Uri(param.AuthPage),
                    param.AuthParam);

                if (page != null)
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(page);
                    HtmlNodeCollection nodesDate = htmlDocument.DocumentNode.SelectNodes(param.XPathDate);
                    HtmlNodeCollection nodesName = htmlDocument.DocumentNode.SelectNodes(param.XPathName);

                    if (nodesDate != null &&
                        nodesName != null)
                    {
                        var ruCultutre = new CultureInfo("RU-ru");
                        var postQuery =
                            from date in nodesDate
                            join name in nodesName on date.Line equals name.Line + 6
                            select new NnmclubListItem
                            {
                                Created = DateTime.Now,
                                Added = DateTime.ParseExact(
                                    HttpUtility.HtmlDecode(
                                        date.InnerText.Remove(0, date.InnerText.Length - param.DateTimeFormat.Length)),
                                    param.DateTimeFormat,
                                    ruCultutre,
                                    DateTimeStyles.AllowInnerWhite),
                                Name = HttpUtility.HtmlDecode(
                                    name.GetAttributeValue("title", null)),
                                Href = name.GetAttributeValue("href", null)
                            };
                        result.AddRange(postQuery.ToList());
                        continue;
                    }
                    _logger.LogError("XPath выражение вернуло null");
                    return null;
                }
                _logger.LogError("При получении веб страницы вернулось null");
                return null;
            }
            return result;
        }

        /// <summary>
        /// Парсит указанную презентацию
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<NnmclubItem> GetItem(NnmclubParseItemInput param)
        {
            NnmclubListItem listItem =
                _context
                .NnmclubListItems
                .SingleOrDefault(el => el.Id == param.ListItemId);

            if (listItem != null)
            {
                string page = await GetPage(
                    param.UriItem + listItem.Href,
                    param.ProxySocks5Addr,
                    param.ProxySocks5Port,
                    param.ProxyActive,
                    new Uri(param.AuthPage),
                    param.AuthParam);
                if (page != null)
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(page);

                    List<NnmclubItemSpoiler> listSpoiler = GetSpoilers(htmlDocument,
                                                                       param.XPathSpoiler);
                    var poster = new StringBuilder(GetPoster(htmlDocument, param.XPathPoster));
                    poster.Remove(0, poster
                          .ToString()
                          .IndexOf("?link=") + "?link=".Length);
                    string torrent = GetTorrent(htmlDocument, param.XPathTorrent);
                    List<NnmclubItemImg> listImgs = GetImgs(htmlDocument,
                                                    param.XPathImgs);
                    string description = GetDescription(htmlDocument,
                                                        param.XPathDescription,
                                                        param.XPathSpoiler,
                                                        param.XPathPoster,
                                                        param.XPathImgs,
                                                        param.XPathTrash);

                    if (description == null || listImgs == null || poster == null)
                    {
                        _logger.LogError($"Не удалось получить описание презентации или ее изображений. NnmclubListItemId - {listItem.Id} / Href - {listItem.Href}");
                        return null;
                    }

                    var item = new NnmclubItem
                    {
                        Created = DateTime.Now,
                        Name = listItem.Name,
                        Description = description,
                        Spoilers = listSpoiler,
                        Poster = poster.ToString(),
                        Imgs = listImgs,
                        Torrent = torrent,
                        NnmclubListItemId = param.ListItemId,
                    };
                    return item;
                }
                _logger.LogError($"Не удалось получить веб страницу с презентацией. NnmclubListItemId - {listItem.Id} / Href - {listItem.Href}");
                return null;
            }
            _logger.LogError("Не удалось найти в базе презентацию из списка с указнным Id");
            return null;
        }

        /// <summary>
        /// Возвращает список спойлеров презентации
        /// </summary>
        /// <param name="doc">Html документ с раздачей</param>
        /// <param name="xPathSpoilers">XPath выражение для поиска споейлеров</param>
        /// <returns>Список спойлеров</returns>
        List<NnmclubItemSpoiler> GetSpoilers(HtmlDocument doc, string xPathSpoilers)
        {
            HtmlNodeCollection nodeSpoilers =
                doc.DocumentNode.SelectNodes(xPathSpoilers);

            if (nodeSpoilers == null)
            {
                _logger.LogError("Не удалось найти спойлеры на странице с презентацией");
                return null;
            }

            var spoilers = new List<NnmclubItemSpoiler>();
            foreach (var spoiler in nodeSpoilers)
            {
                var itemCollect = new NnmclubItemSpoiler()
                {
                    Header = HttpUtility.HtmlDecode(spoiler.GetAttributeValue("title", null)),
                    Body = spoiler.InnerHtml,
                    Created = DateTime.Now
                };
                spoilers.Add(itemCollect);
            }
            return spoilers;
        }

        /// <summary>
        /// Возвращает ссылку на постер презентации
        /// </summary>
        /// <param name="doc">html документ с возможным постером</param>
        /// <param name="xPathPoster">xpath для постера</param>
        /// <returns></returns>
        string GetPoster(HtmlDocument doc, string xPathPoster)
        {
            HtmlNode posterNode = doc.DocumentNode.SelectSingleNode(xPathPoster);
            if (posterNode != null)
            {
                return posterNode.GetAttributeValue("title", null);
            }
            return null;
        }

        /// <summary>
        /// Получить ссылку на торрент файл
        /// </summary>
        /// <param name="doc">Html страница с презентацией</param>
        /// <param name="xPathTorrent">XPath выражение для парсинга торрент файла</param>
        /// <returns>Ссылка на файл</returns>
        string GetTorrent(HtmlDocument doc, string xPathTorrent)
        {
            HtmlNode posterNode = doc.DocumentNode.SelectSingleNode(xPathTorrent);
            if (posterNode != null)
            {
                return posterNode.GetAttributeValue("href", null);
            }
            return null;
        }

        /// <summary>
        /// Возвращает список прямых ссылок на скриншоты презентации
        /// </summary>
        /// <param name="doc">Html докумет с презентацией</param>
        /// <param name="xPathImgs">XPath выражение для поиска изображений</param>
        /// <returns></returns>
        List<NnmclubItemImg> GetImgs(HtmlDocument doc,
                             string xPathImgs)
        {
            HtmlNodeCollection nodeImgs =
                doc.DocumentNode.SelectNodes(xPathImgs);

            if (nodeImgs != null)
            {
                List<NnmclubItemImg> imgsList =
                    (from img in nodeImgs
                     select new NnmclubItemImg
                     {
                         Created = DateTime.Now,
                         ImgUri = img.GetAttributeValue("href", null)
                     }).ToList();
                return imgsList;
            }

            _logger.LogError("Не удалось найти ни одного скриншота на странице презентации");
            return null;
        }

        /// <summary>
        /// Возвращает описание презентации
        /// </summary>
        /// <param name="doc">Html документ с презентацией</param>
        /// <returns>Строка с описанием</returns>
        string GetDescription(HtmlDocument doc, 
                              string xPathDesc, 
                              string xPathSpoilers, 
                              string xPathPoster, 
                              string xPathImgs, 
                              string xPathTrash)
        {
            HtmlNode nodeDescription =
                doc.DocumentNode.SelectSingleNode(xPathDesc);
            HtmlNodeCollection nodeSpoilersRemove =
                nodeDescription.SelectNodes(xPathSpoilers);
            HtmlNode nodePosterRemove =
                nodeDescription.SelectSingleNode(xPathPoster);
            HtmlNodeCollection nodeImgsRemove =
                nodeDescription.SelectNodes(xPathImgs);

            if (nodeDescription == null)
            {
                _logger.LogError("Не удалось найти описание презентации по указанному XPath выражению");
                return null;
            }

            if (nodeSpoilersRemove != null)
                foreach (var item in nodeSpoilersRemove)
                {
                    item.Remove();
                }
            if (nodeImgsRemove != null)
                foreach (var item in nodeImgsRemove)
                {
                    item.Remove();
                }
            if (nodePosterRemove != null)
                nodePosterRemove.Remove();

            HtmlNodeCollection htmlNodes = nodeDescription.SelectNodes(xPathTrash);
            if (htmlNodes != null)
                foreach (HtmlNode node in htmlNodes)
                {
                    node.Remove();
                }

            return nodeDescription.InnerHtml;
        }

        /// <summary>
        /// Возвращает веб страницу, загрузка через тор прокси
        /// </summary>
        /// <param name="uri">Адрес страницы</param>
        /// <param name="address">Socks5 адрес прокси</param>
        /// <param name="port">Порт прокси</param>
        /// <param name="usingProxy">Использовать или нет тор прокси</param>
        /// <returns>Веб страница, если произошла ошибка возвращает null</returns>
        async Task<string> GetPage(string uri, string address, int port, bool usingProxy, Uri authPage, string authParam)
        {
            var webClient = new WebClientAwareCookie();
            if (usingProxy)
                webClient.Proxy = new HttpToSocks5Proxy(address, port);
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            await webClient.UploadStringTaskAsync(authPage, authParam);

            byte[] data;
            try
            {
                data = await webClient.DownloadDataTaskAsync(new Uri(uri));
            }
            catch (WebException ex)
            {
                _logger.LogError(ex, "Указан неврный адрес или произошла другая сетевая ошибка");
                return null;
            }

            if (data != null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding utf8 = Encoding.GetEncoding("UTF-8");
                Encoding win1251 = Encoding.GetEncoding("windows-1251");
                data = Encoding.Convert(win1251, utf8, data);
                return utf8.GetString(data);
            }
            _logger.LogError("После загрузки вернулось null");
            return null;
        }
        #endregion
    }
}
