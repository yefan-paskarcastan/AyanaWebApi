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
