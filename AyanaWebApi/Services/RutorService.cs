using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Web;

using HtmlAgilityPack;
using MihaZupan;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services
{
    /// <summary>
    /// Парсит раздачи с рутора
    /// </summary>
    public class RutorService : IRutorService
    {
        public RutorService(AyDbContext ayDbContext)
        {
            _context = ayDbContext;
        }

        public async Task<RutorItem> ParseItem(RutorParseItemInput param)
        {
            RutorListItem listItem = 
                _context.
                RutorListItems.
                SingleOrDefault(el => el.Id == param.ListItemId);
            if (listItem != null)
            {
                string page = await GetPage(param.UriItem + listItem.HrefNumber, param.ProxySocks5Addr, param.ProxySocks5Port);
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

                    if (description == null 
                        || listSpoiler == null
                        || listImgs == null) return null;

                    var rutorItem = new RutorItem
                    {
                        Description = description,
                        Spoilers = listSpoiler,
                        Imgs = listImgs
                    };

                    return rutorItem;
                }
            }
            return null;
        }

        /// <summary>
        /// Возвращает список найденных раздач на странице со списком
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<IList<RutorListItem>> CheckListSettings(RutorCheckListInput param)
        {
            return await GetListItems(param);
        }

        /// <summary>
        /// Проверит список раздач рутора и записать в базу
        /// </summary>
        /// <param name="uri">Ссылка на список рутора</param>
        /// <param name="xpathExp">Выражение xpath для парсинга</param>
        /// <returns></returns>
        public async Task<Log> CheckList(RutorCheckListInput param)
        {
            IList<RutorListItem> items = await GetListItems(param);
            if (items != null)
            {
                int newPostCount;
                if (_context.RutorListItems.Count() == 0)
                {
                    await _context.RutorListItems.AddRangeAsync(items);
                    newPostCount = await _context.SaveChangesAsync();
                    return new Log()
                    {
                        Created = DateTime.Now,
                        Message = $"{newPostCount} новых записей успешно добавлены в бд"
                    };
                }
                IList<RutorListItem> oldItems = 
                    _context
                    .RutorListItems
                    .OrderByDescending(d => d.Id)
                    .Take(100)
                    .ToList();
                IList<RutorListItem> onlyNew = 
                    items
                    .Except(oldItems, new RutorListItemComaprer())
                    .ToList();

                await _context.RutorListItems.AddRangeAsync(onlyNew);
                newPostCount = await _context.SaveChangesAsync();
                return new Log()
                {
                    Created = DateTime.Now,
                    Message = $"{newPostCount} новых записей успешно добавлены в бд"
                };
            }
            else
            {
                return new Log()
                {
                    Created = DateTime.Now,
                    Message = "При загрузке страницы со списком раздач вернулось null.",
                };
            }
        }

        #region Private
        readonly AyDbContext _context;

        /// <summary>
        /// Возвращает веб страницу, загрузка через тор прокси
        /// </summary>
        /// <param name="uri">Адрес страницы</param>
        /// <param name="address">Socks5 адрес прокси</param>
        /// <param name="port">Порт прокси</param>
        /// <returns>Веб страница</returns>
        async Task<string> GetPage(string uri, string address, int port)
        {
            var webClient = new WebClient();
            webClient.Proxy = new HttpToSocks5Proxy(address, port);

            byte[] data;
            data = await webClient.DownloadDataTaskAsync(new Uri(uri));

            if (data != null)
            {
                return Encoding.UTF8.GetString(data);
            }
            return null;
        }

        /// <summary>
        /// Возвращает список найденных раздач на странице со списком
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<IList<RutorListItem>> GetListItems(RutorCheckListInput param)
        {
            string page = await GetPage(param.UriList, param.ProxySocks5Addr, param.ProxySocks5Port);
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
                    var postQuery =
                        from date in nodesDate
                        join number in nodesUniqNumber on date.Line equals number.Line
                        join name in nodesName on date.Line equals name.Line - 1
                        select new RutorListItem
                        {
                            AddedDate = HttpUtility.HtmlDecode(date.InnerText),
                            HrefNumber =
                                number.GetAttributeValue("href", null).
                                Split(param.XPathParamSplitSeparator)[param.XPathParamSplitIndex],
                            Name = HttpUtility.HtmlDecode(name.InnerText),
                        };
                    return postQuery.Reverse().ToList();
                }
                await _context.Logs.AddAsync(
                    new Log()
                    {
                        Created = DateTime.Now,
                        Message = "XPath выражение вернуло null. Нужные объекты не найдены.",
                    });
                await _context.SaveChangesAsync();
            }
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

            if (nodeDescription == null) return null;
            if (nodeSpoilersRemove == null) return null;

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

            if (nodeSpoilers == null) return null;
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

            if (nodeImgs == null) return null;

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
