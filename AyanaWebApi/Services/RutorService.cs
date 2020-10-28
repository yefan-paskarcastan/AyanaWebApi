using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Web;
using System.Globalization;

using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using MihaZupan;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Services
{
    /// <summary>
    /// Парсит раздачи с рутора
    /// </summary>
    public class RutorService : IRutorService
    {
        readonly AyDbContext _context;
        readonly ILogger<RutorService> _logger;

        public RutorService(AyDbContext ayDbContext,
                            ILogger<RutorService> logger)
        {
            _context = ayDbContext;
            _logger = logger;
        }

        /// <summary>
        /// Парсит указанную раздачу и записывает ее в базу
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<RutorItem> ParseItem(RutorParseItemInput param)
        {
            RutorItem item = await ParsePageItem(param);

            if (item != null)
            {
                _context.RutorItems.Add(item);
                _context.SaveChanges();
                return item;
            }
            _logger.LogError("Не удалось распрарсить раздачу");
            return null;
        }

        /// <summary>
        /// Проверит список раздач рутора и вернуть новые
        /// </summary>
        /// <returns></returns>
        public async Task<IList<RutorListItem>> CheckList(RutorCheckListInput param)
        {
            IList<RutorListItem> items = await GetListItems(param);

            if (items != null)
            {
                if (_context.RutorListItems.Count() == 0)
                {
                    await _context.RutorListItems.AddRangeAsync(items);
                    await _context.SaveChangesAsync();
                    return items;
                }
                IList<RutorListItem> oldItems = 
                    _context
                    .RutorListItems
                    .OrderByDescending(d => d.AddedDate)
                    .Take(200)
                    .ToList();
                IList<RutorListItem> onlyNew = 
                    items.Except(oldItems, new RutorListItemComparer())
                    .ToList();

                await _context.RutorListItems.AddRangeAsync(onlyNew);
                await _context.SaveChangesAsync();
                items = onlyNew;
                return items;
            }
            _logger.LogError("При получении полного списка раздач вернулось null");
            return null;
        }

        #region Private
        /// <summary>
        /// Парсит указанную раздачу
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<RutorItem> ParsePageItem(RutorParseItemInput param)
        {
            RutorListItem listItem =
                _context
                .RutorListItems
                .SingleOrDefault(el => el.Id == param.ListItemId);

            if (listItem != null)
            {
                string page = await GetPage(param.UriItem + listItem.HrefNumber, 
                                            param.ProxySocks5Addr, 
                                            param.ProxySocks5Port,
                                            param.ProxyActive);
                if (page != null)
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(page);

                    List<RutorItemSpoiler> listSpoiler = GetSpoilers(htmlDocument,
                                                                     param.XPathExprSpoiler);
                    List<RutorItemImg> listImgs = GetImgs(htmlDocument,
                                                          param.XPathExprImgs,
                                                          listSpoiler);
                    string description = GetDescription(htmlDocument,
                                                        param.XPathExprDescription,
                                                        param.XPathExprSpoiler);

                    if (description == null || listImgs == null)
                    {
                        _logger.LogError($"Не удалось получить описание презентации или ее изображения. RutorListItem.Id - {listItem.Id} / Href - {listItem.HrefNumber}");
                        return null;
                    }

                    var result =  new RutorItem
                    {
                        Created = DateTime.Now,
                        Name = listItem.Name,
                        Description = description,
                        Spoilers = listSpoiler,
                        Imgs = listImgs,
                        RutorListItemId = param.ListItemId,
                    };
                    return result;
                }
                _logger.LogError($"Не удалось получить веб страницу с презентацией. RutorListItem.Id - {listItem.Id} / Href - {listItem.HrefNumber}");
                return null;
            }
            _logger.LogError("Не удалось найти в базе раздачу из списка с указнным Id");
            return null;
        }

        /// <summary>
        /// Возвращает веб страницу, загрузка через тор прокси
        /// </summary>
        /// <param name="uri">Адрес страницы</param>
        /// <param name="address">Socks5 адрес прокси</param>
        /// <param name="port">Порт прокси</param>
        /// <param name="usingProxy">Использовать или нет тор прокси</param>
        /// <returns>Веб страница, если произошла ошибка возвращает null</returns>
        async Task<string> GetPage(string uri, string address, int port, bool usingProxy)
        {
            var webClient = new WebClient();
            if(usingProxy)
                webClient.Proxy = new HttpToSocks5Proxy(address, port);

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
                return Encoding.UTF8.GetString(data);
            return null;
        }

        /// <summary>
        /// Возвращает список найденных раздач на странице со списком
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<IList<RutorListItem>> GetListItems(RutorCheckListInput param)
        {
            string page = await GetPage(param.UriList, 
                                        param.ProxySocks5Addr, 
                                        param.ProxySocks5Port,
                                        param.ProxyActive);
            if (page != null)
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(page);
                HtmlNodeCollection nodesDate = htmlDocument.DocumentNode.SelectNodes(param.XPathExprItemDate);
                HtmlNodeCollection nodesUniqNumber = htmlDocument.DocumentNode.SelectNodes(param.XPathExprItemUniqNumber);
                HtmlNodeCollection nodesName = htmlDocument.DocumentNode.SelectNodes(param.XPathExprItemName);

                if (nodesDate != null &&
                    nodesUniqNumber != null &&
                    nodesName != null)
                {
                    var ruCultutre = new CultureInfo("RU-ru");                    
                    var postQuery =
                        from date in nodesDate
                        join number in nodesUniqNumber on date.Line equals number.Line
                        join name in nodesName on date.Line equals name.Line - 1
                        select new RutorListItem
                        {
                            Created = DateTime.Now,
                            AddedDate = DateTime.ParseExact(HttpUtility.HtmlDecode(date.InnerText), 
                                                            "dd MMM yy", 
                                                            ruCultutre, 
                                                            DateTimeStyles.AllowInnerWhite),
                            HrefNumber =
                                number.GetAttributeValue("href", null).
                                Split(param.XPathParamSplitSeparator)[param.XPathParamSplitIndex],
                            Name = HttpUtility.HtmlDecode(name.InnerText),
                        };
                    return postQuery.ToList();
                }
                _logger.LogError("XPath выражение вернуло null");
                return null;
            }
            _logger.LogError("При получении веб страницы вернулось null");
            return null;
        }

        /// <summary>
        /// Возвращает описание раздачи
        /// </summary>
        /// <param name="doc">Html документ с раздачей</param>
        /// <returns>Строка с описанием</returns>
        string GetDescription(HtmlDocument doc, string xPathDesc, string xPathSpoilers)
        {
            HtmlNode nodeDescription =
                doc.DocumentNode.SelectSingleNode(xPathDesc);
            HtmlNodeCollection nodeSpoilersRemove =
                nodeDescription.SelectNodes(xPathSpoilers);

            if (nodeDescription == null)
            {
                _logger.LogError("Не удалось найти описание раздачи по указанному XPath выражению");
                return null;
            }

            if (nodeSpoilersRemove != null)
            foreach (var item in nodeSpoilersRemove)
            {
                item.Remove();
            }

            return nodeDescription.InnerHtml;
        }

        /// <summary>
        /// Возвращает список спойлеров раздачи
        /// </summary>
        /// <param name="doc">Html документ с раздачей</param>
        /// <param name="xPathSpoilers">XPath выражение для поиска споейлеров</param>
        /// <returns>Список спойлеров</returns>
        List<RutorItemSpoiler> GetSpoilers(HtmlDocument doc, string xPathSpoilers)
        {
            HtmlNodeCollection nodeSpoilers =
                doc.DocumentNode.SelectNodes(xPathSpoilers);

            if (nodeSpoilers == null) 
            {
                _logger.LogError("Не удалось найти спойлеры на странице с раздачей");
                return null; 
            }

            var spoilers = new List<RutorItemSpoiler>();
            foreach (var spoiler in nodeSpoilers)
            {
                var itemCollect = new RutorItemSpoiler()
                {
                    Header = spoiler.FirstChild.InnerText,
                    Body = spoiler.LastChild.InnerText,
                    Created = DateTime.Now
                };
                spoilers.Add(itemCollect);
            }
            return spoilers;
        }

        /// <summary>
        /// Возвращает список найденных изображений, относящиеся к раздаче
        /// </summary>
        /// <param name="doc">Html докумет с раздачей</param>
        /// <param name="xPathImgs">XPath выражение для поиска изображений</param>
        /// <param name="listSpoiler">Список спойлеров, в которых возможно есть изорбражения</param>
        /// <returns></returns>
        List<RutorItemImg> GetImgs(HtmlDocument doc, 
                                   string xPathImgs, 
                                   List<RutorItemSpoiler> listSpoiler)
        {
            var imgsSpoilers = new List<RutorItemImg>();
            if (listSpoiler != null)
                foreach (var item in listSpoiler)
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(item.Body);
                    HtmlNodeCollection nodes = document.DocumentNode.SelectNodes(@"//img");
                    if (nodes != null)
                    {
                        foreach (var img in nodes)
                        {
                            imgsSpoilers.Add(
                                new RutorItemImg
                                {
                                    ParentUrl = img.ParentNode.GetAttributeValue("href", null),
                                    ChildUrl = img.GetAttributeValue("src", null),
                                    Created = DateTime.Now,
                                });
                        }
                    }
                }

            HtmlNodeCollection nodeImgs =
                doc.DocumentNode.SelectNodes(xPathImgs);

            if (nodeImgs == null)
            {
                _logger.LogError("Не удалось найти ни одного изображения в описании раздачи");
                return null; 
            }

            List<RutorItemImg> list =
                (from img in nodeImgs
                 select new RutorItemImg
                 {
                     ParentUrl = img.ParentNode.GetAttributeValue("href", null),
                     ChildUrl = img.GetAttributeValue("src", null),
                     Created = DateTime.Now,
                 }).ToList();

            list.AddRange(imgsSpoilers);
            return list;
        }
        #endregion
    }
}
