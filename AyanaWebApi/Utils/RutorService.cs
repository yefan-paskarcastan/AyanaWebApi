using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MihaZupan;
using AyanaWebApi.Models;
using System.Net;
using AyanaWebApi.ApiEntities;
using System.Text;
using HtmlAgilityPack;
using System.Web;

namespace AyanaWebApi.Utils
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

        public async Task<RutorItem> ParseItem(RutorInputParseItem param)
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
                    HtmlNode nodeDescription =
                        htmlDocument.DocumentNode.SelectSingleNode(param.XPathExprDescription);
                    HtmlNodeCollection nodeSpoilersRemove =
                        nodeDescription.SelectNodes(param.XPathExprSpoiler);
                    HtmlNodeCollection nodeSpoilers =
                        htmlDocument.DocumentNode.SelectNodes(param.XPathExprSpoiler);

                    foreach (var item in nodeSpoilersRemove)
                    {
                        item.Remove();
                    }

                    var rutorItem = new RutorItem();

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
                    rutorItem.Description = nodeDescription.InnerHtml;
                    rutorItem.Spoilers = spoilers;

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
        public async Task<IList<RutorListItem>> CheckListSettings(RutorCheckList param)
        {
            return await GetListItems(param);
        }

        /// <summary>
        /// Проверит список раздач рутора и записать в базу
        /// </summary>
        /// <param name="uri">Ссылка на список рутора</param>
        /// <param name="xpathExp">Выражение xpath для парсинга</param>
        /// <returns></returns>
        public async Task<ParseResult> CheckList(RutorCheckList param)
        {
            IList<RutorListItem> items = await GetListItems(param);
            if (items != null)
            {
                int newPostCount;
                if (_context.RutorListItems.Count() == 0)
                {
                    await _context.RutorListItems.AddRangeAsync(items);
                    newPostCount = await _context.SaveChangesAsync();
                    return new ParseResult()
                    {
                        Created = DateTime.Now,
                        Success = true,
                        Message = $"{newPostCount} новых записей успешно добавлены в бд"
                    };
                }
                IList<RutorListItem> oldItems = 
                    _context.RutorListItems.
                    OrderByDescending(d => d.Id).Take(100).ToList();
                IList<RutorListItem> onlyNew = 
                    items.Except(oldItems, new RutorListItemComaprer()).ToList();

                await _context.RutorListItems.AddRangeAsync(onlyNew);
                newPostCount = await _context.SaveChangesAsync();
                return new ParseResult()
                {
                    Created = DateTime.Now,
                    Success = true,
                    Message = $"{newPostCount} новых записей успешно добавлены в бд"
                };
            }
            else
            {
                return new ParseResult()
                {
                    Created = DateTime.Now,
                    Message = "При загрузке страницы со списком раздач вернулось null.",
                    Success = false
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
            try
            {
                data = await webClient.DownloadDataTaskAsync(new Uri(uri));
            }
            catch (WebException ex)
            {
                var result = new ParseResult()
                {
                    Created = DateTime.Now,
                    Message = $"WebException при загрузке страницы со списком разадач. {ex.Message}",
                    Success = false
                };
                await _context.ParseResults.AddAsync(result);
                await _context.SaveChangesAsync();
                return null;
            }
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
        async Task<IList<RutorListItem>> GetListItems(RutorCheckList param)
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
                await _context.ParseResults.AddAsync(
                    new ParseResult()
                    {
                        Created = DateTime.Now,
                        Message = "XPath выражение вернуло null. Нужные объекты не найдены.",
                        Success = false
                    });
                await _context.SaveChangesAsync();
            }
            return null;
        }
        #endregion
    }
}
