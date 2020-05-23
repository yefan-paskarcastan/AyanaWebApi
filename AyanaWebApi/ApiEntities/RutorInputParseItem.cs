using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.ApiEntities
{
    public class RutorInputParseItem
    {
        /// <summary>
        /// Шаблон адреса страницы с раздачей
        /// </summary>
        public string UriItem { get; set; }

        /// <summary>
        /// Адрес тор прокси
        /// </summary>
        public string ProxySocks5Addr { get; set; }

        /// <summary>
        /// Порт тор прокси
        /// </summary>
        public int ProxySocks5Port { get; set; }

        /// <summary>
        /// XPath выражение для парсинга описания
        /// </summary>
        public string XPathExprDescription { get; set; }

        /// <summary>
        /// XPath выражение для парсинга спойлеров
        /// </summary>
        public string XPathExprSpoiler { get; set; }

        /// <summary>
        /// Id объекта листа, по которму будет строится адрес страницы
        /// </summary>
        public int ListItemId { get; set; }
    }
}
