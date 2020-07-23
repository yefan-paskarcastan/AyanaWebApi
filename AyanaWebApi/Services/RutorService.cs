using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Web;
using System.Globalization;

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
        public RutorService(AyDbContext ayDbContext,
                            ILogService logService)
        {
            _context = ayDbContext;
            _logs = logService;
        }

        /// <summary>
        /// Парсит указанную раздачу и записывает ее в базу
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<ServiceResult<RutorItem>> ParseItem(RutorParseItemInput param)
        {
            ServiceResult<RutorItem> item = await ParsePageItem(param);
            item.ServiceName = nameof(RutorService);
            item.Location = "Парсинг раздачи";

            if (item.ResultObj != null)
            {
                _context.RutorItems.Add(item.ResultObj);
                _context.SaveChanges();
                return item;
            }

            item.Comment = "Не удалось распрарсить раздачу";
            _logs.Write(item);
            return item;
        }

        /// <summary>
        /// Проверит список раздач рутора и вернуть новые
        /// </summary>
        /// <returns></returns>
        public async Task<ServiceResult<IList<RutorListItem>>> CheckList(RutorCheckListInput param)
        {
            ServiceResult<IList<RutorListItem>> items = await GetListItems(param);
            items.ServiceName = nameof(RutorService);
            items.Location = "Работа с новым и существующим списком раздач";

            if (items.ResultObj != null)
            {
                if (_context.RutorListItems.Count() == 0)
                {
                    await _context.RutorListItems.AddRangeAsync(items.ResultObj);
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
                    items.ResultObj
                    .Except(oldItems, new RutorListItemComaprer())
                    .ToList();

                await _context.RutorListItems.AddRangeAsync(onlyNew);
                await _context.SaveChangesAsync();
                items.ResultObj = onlyNew;
                return items;
            }
            items.Comment = "При получении полного списка раздач вернулось null";
            _logs.Write(items);
            return items;
        }

        #region Private
        readonly AyDbContext _context;

        readonly ILogService _logs;

        /// <summary>
        /// Парсит указанную раздачу
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<ServiceResult<RutorItem>> ParsePageItem(RutorParseItemInput param)
        {
            var result = new ServiceResult<RutorItem>();
            result.ServiceName = nameof(RutorService);
            result.Location = "Парсинг раздачи";

            RutorListItem listItem =
                _context
                .RutorListItems
                .SingleOrDefault(el => el.Id == param.ListItemId);

            if (listItem != null)
            {
                ServiceResult<string> page = await GetPage(param.UriItem + listItem.HrefNumber, param.ProxySocks5Addr, param.ProxySocks5Port);
                if (page.ResultObj != null)
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(page.ResultObj);

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
                        result.Comment = "Не удалось получить описани раздачи или ее изображения";
                        result.ErrorContent = $"RutorListItemId - {listItem.Id} / Href - {listItem.HrefNumber}";
                        _logs.Write(result);
                        return result;
                    }

                    result.ResultObj =  new RutorItem
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
                result.Comment = "Не удалось получить веб страницу с раздачей";
                result.ErrorContent = result.ErrorContent = $"RutorListItemId - {listItem.Id} / Href - {listItem.HrefNumber}";
                _logs.Write(result);
                return result;
            }
            result.Comment = "Не удалось найти в базе раздачу из списка с указнным Id";
            _logs.Write(result);
            return result;
        }

        /// <summary>
        /// Возвращает веб страницу, загрузка через тор прокси
        /// </summary>
        /// <param name="uri">Адрес страницы</param>
        /// <param name="address">Socks5 адрес прокси</param>
        /// <param name="port">Порт прокси</param>
        /// <returns>Веб страница, если произошла ошибка возвращает null</returns>
        async Task<ServiceResult<string>> GetPage(string uri, string address, int port)
        {
            var result = new ServiceResult<string>();
            var webClient = new WebClient();
            webClient.Proxy = new HttpToSocks5Proxy(address, port);

            byte[] data = null;
            try
            {
                data = await webClient.DownloadDataTaskAsync(new Uri(uri));
            }
            catch (WebException ex)
            {
                result.ServiceName = nameof(RutorService);
                result.Comment = "Указан неврный адрес или произошла другая сетевая ошибка";
                result.Location = "Загрузка веб страницы";
                result.ExceptionMessage = ex.Message;
                _logs.Write(result);
                return result;
            }
            
            if (data != null) result.ResultObj = Encoding.UTF8.GetString(data);
            return result;
        }

        /// <summary>
        /// Возвращает список найденных раздач на странице со списком
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<ServiceResult<IList<RutorListItem>>> GetListItems(RutorCheckListInput param)
        {
            var result = new ServiceResult<IList<RutorListItem>>();
            result.ServiceName = nameof(RutorService);
            result.Location = "Получение полного списка раздач с рутора";

            ServiceResult<string> page = await GetPage(param.UriList, param.ProxySocks5Addr, param.ProxySocks5Port);
            if (page.ResultObj != null)
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(page.ResultObj);
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
                    result.ResultObj = postQuery.ToList();
                    return result;
                }
                result.Comment = "XPath выражение вернуло null";
                _logs.Write(result);
                return result;
            }
            result.Comment = "При получении веб страницы вернулось null";
            _logs.Write(result);
            return result;
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
                var srResult = new ServiceResult<string>
                {
                    ServiceName = nameof(RutorService),
                    Location = "Парсинг описания раздачи",
                    Comment = "Не удалось найти описание раздачи по указанному XPath выражению"
                };
                _logs.Write(srResult);
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
                var srResult = new ServiceResult<string>
                {
                    ServiceName = nameof(RutorService),
                    Location = "Парсинг раздачи / Парсинг спойлеров",
                    Comment = "Не удалось найти спойлеры на странице с раздачей"
                };
                _logs.Write(srResult);
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
                var srResult = new ServiceResult<string>
                {
                    ServiceName = nameof(RutorService),
                    Location = "Парсинг раздачи / Получение изображений из описания",
                    Comment = "Не удалось найти ни одного изображения в описании раздачи"
                };
                _logs.Write(srResult);
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
