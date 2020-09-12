using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;

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
        public async Task<ServiceResult<IList<RutorListItem>>> CheckList(NnmclubCheckListInput param)
        {
            ServiceResult<IList<RutorListItem>> items = await GetList(param);
            items.ServiceName = nameof(NnmclubService);
            items.Location = "Проверка списка клуба";

            if (items.ResultObj != null)
            {

            }
            items.Comment = "При получении полного списка презентаций кулба вернулось null";
            _logs.Write(items);
            return items;
        }

        #region Private
        readonly AyDbContext _context;
        readonly ILogService _logs;

        /// <summary>
        /// Получает последние 100 презентаций клуба
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        async Task<ServiceResult<IList<RutorListItem>>> GetList(NnmclubCheckListInput param)
        {
            var result = new ServiceResult<IList<RutorListItem>>();
            result.ServiceName = nameof(NnmclubService);
            result.Location = "Получение полного списка презентаций";

            ServiceResult<string> page = await GetPage(param.UriList,
                        param.ProxySocks5Addr,
                        param.ProxySocks5Port,
                        param.ProxyActive);

            if (page.ResultObj != null)
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(page.ResultObj);
                HtmlNodeCollection nodesDate = htmlDocument.DocumentNode.SelectNodes(param.XPathDate);
                if (nodesDate != null)
                {

                }
            }
            result.Comment = "При получении веб страницы вернулось null";
            _logs.Write(result);
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

            if (data != null) result.ResultObj = Encoding.UTF8.GetString(data);
            return result;
        }
        #endregion
    }
}
