using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Globalization;
using System.Web;

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
        public NnmclubService(AyDbContext ayDbContext,
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
        public async Task<ServiceResult<NnmclubItem>> ParseItem(NnmclubParseItemInput param)
        {
            ServiceResult<NnmclubItem> item = await GetItem(param);
            item.ServiceName = nameof(NnmclubService);
            item.Location = "Парсинг презентации";

            if (item.ResultObj != null)
            {
                _context.NnmclubItems.Add(item.ResultObj);
                _context.SaveChanges();
                return item;
            }

            item.Comment = "Не удалось распрарсить презентацию";
            _logs.Write(item);
            return item;
        }

        /// <summary>
        /// Проверить список презентаций клуба
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<ServiceResult<IList<NnmclubListItem>>> CheckList(NnmclubCheckListInput param)
        {
            ServiceResult<IList<NnmclubListItem>> items = await GetList(param);
            items.ServiceName = nameof(NnmclubService);
            items.Location = "Проверка списка клуба";

            if (items.ResultObj != null)
            {
                if (_context.NnmclubListItems.Count() == 0)
                {
                    _context.NnmclubListItems.AddRange(items.ResultObj);
                    _context.SaveChanges();
                    return items;
                }
                IList<NnmclubListItem> oldItems =
                    _context
                    .NnmclubListItems
                    .OrderByDescending(d => d.Added)
                    .Take(200)
                    .ToList();
                IList<NnmclubListItem> onlyNew =
                    items.ResultObj
                    .Except(oldItems, new NnmclubListItemComparer())
                    .ToList();

                _context.NnmclubListItems.AddRange(onlyNew);
                _context.SaveChanges();
                items.ResultObj = onlyNew;
                return items;
            }
            items.Comment = "При получении полного списка презентаций вернулось null";
            _logs.Write(items);
            return items;
        }

        #region Private
        readonly AyDbContext _context;
        readonly ILogService _logs;

        /// <summary>
        /// Получает последние презентации клуба
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<ServiceResult<IList<NnmclubListItem>>> GetList(NnmclubCheckListInput param)
        {
            var result = new ServiceResult<IList<NnmclubListItem>>();
            result.ServiceName = nameof(NnmclubService);
            result.Location = "Получение полного списка презентаций";
            var tmpList = new List<NnmclubListItem>();

            for (int i = param.UriListCount; i >= 0; i--)
            {
                ServiceResult<string> page = await GetPage(
                    param.UriList + (i * param.UriListIncrement),
                    param.ProxySocks5Addr,
                    param.ProxySocks5Port,
                    param.ProxyActive);

                    if (page.ResultObj != null)
                    {
                        var htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(page.ResultObj);
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
                            tmpList.AddRange(postQuery.ToList());
                            continue;
                        }
                        result.Comment = "XPath выражение вернуло null";
                        _logs.Write(result);
                        return result;
                    }
                    result.Comment = "При получении веб страницы вернулось null";
                    _logs.Write(result);
                    return result;
            }
            result.ResultObj = tmpList;
            return result;
        }

        /// <summary>
        /// Парсит указанную презентацию
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<ServiceResult<NnmclubItem>> GetItem(NnmclubParseItemInput param)
        {
            var result = new ServiceResult<NnmclubItem>();
            result.ServiceName = nameof(NnmclubService);
            result.Location = "Парсинг презентации";

            NnmclubListItem listItem =
                _context
                .NnmclubListItems
                .SingleOrDefault(el => el.Id == param.ListItemId);

            if (listItem != null)
            {
                ServiceResult<string> page = await GetPage(param.UriItem + listItem.Href,
                    param.ProxySocks5Addr,
                    param.ProxySocks5Port,
                    param.ProxyActive);
                if (page.ResultObj != null)
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(page.ResultObj);

                    List<NnmclubItemSpoiler> listSpoiler = GetSpoilers(htmlDocument,
                                                                       param.XPathSpoiler);
                    string poster = GetPoster(htmlDocument, param.XPathPoster);
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
                        result.Comment = "Не удалось получить описание презентации или ее изображений";
                        result.ErrorContent = $"NnmclubListItemId - {listItem.Id} / Href - {listItem.Href}";
                        _logs.Write(result);
                        return result;
                    }

                    result.ResultObj = new NnmclubItem
                    {
                        Created = DateTime.Now,
                        Name = listItem.Name,
                        Description = description,
                        Spoilers = listSpoiler,
                        Poster = poster,
                        Imgs = listImgs,
                        NnmclubListItemId = param.ListItemId,
                    };
                    return result;
                }
                result.Comment = "Не удалось получить веб страницу с презентацией";
                result.ErrorContent = result.ErrorContent = $"NnmclubListItemId - {listItem.Id} / Href - {listItem.Href}";
                _logs.Write(result);
                return result;
            }
            result.Comment = "Не удалось найти в базе презентацию из списка с указнным Id";
            _logs.Write(result);
            return result;
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
                var srResult = new ServiceResult<string>
                {
                    ServiceName = nameof(NnmclubService),
                    Location = "Парсинг презентации / Парсинг спойлеров",
                    Comment = "Не удалось найти спойлеры на странице с презентацией"
                };
                _logs.Write(srResult);
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

            var srResult = new ServiceResult<string>
            {
                ServiceName = nameof(NnmclubService),
                Location = "Парсинг презентации / Получение списка скриншотов",
                Comment = "Не удалось найти ни одного скриншота на странице презентации"
            };
            _logs.Write(srResult);
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
                              List<string> xPathTrash)
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
                var srResult = new ServiceResult<string>
                {
                    ServiceName = nameof(NnmclubService),
                    Location = "Парсинг описания презентации",
                    Comment = "Не удалось найти описание презентации по указанному XPath выражению"
                };
                _logs.Write(srResult);
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

            foreach (string item in xPathTrash)
            {
                HtmlNodeCollection htmlNodes = nodeDescription.SelectNodes(item);
                if (htmlNodes != null)
                    foreach (HtmlNode node in htmlNodes)
                    {
                        node.Remove();
                    }
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
        async Task<ServiceResult<string>> GetPage(string uri, string address, int port, bool usingProxy)
        {
            var result = new ServiceResult<string>();
            var webClient = new WebClient();
            if (usingProxy)
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

            if (data != null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding utf8 = Encoding.GetEncoding("UTF-8");
                Encoding win1251 = Encoding.GetEncoding("windows-1251");
                data = Encoding.Convert(win1251, utf8, data);
                result.ResultObj = utf8.GetString(data);
            }
            return result;
        }
        #endregion
    }
}
