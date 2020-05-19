using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MihaZupan;
using AyanaWebApi.Models;
using System.Net;
using AyanaWebApi.ApiEntities;
using System.Text;

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

        /// <summary>
        /// Проверит список раздач рутора и записать в базу
        /// </summary>
        /// <param name="uri">Ссылка на список рутора</param>
        /// <param name="xpathExp">Выражение xpath для парсинга</param>
        /// <returns></returns>
        public async Task<ParseResult> CheckList(RutorCheckList rCheckList)
        {
            string page = await GetPage(rCheckList.UriList, rCheckList.ProxySocks5Addr, rCheckList.ProxySocks5Port);
            if (page != null)
            {
                return new ParseResult()
                {
                    Created = DateTime.Now,
                    Message = page,
                    Success = true
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
        #endregion
    }
}
