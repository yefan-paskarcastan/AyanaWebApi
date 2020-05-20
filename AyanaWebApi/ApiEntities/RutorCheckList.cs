namespace AyanaWebApi.ApiEntities
{
    /// <summary>
    /// Парамерты для проверки списка разадач
    /// </summary>
    public class RutorCheckList
    {
        /// <summary>
        /// Адрес страницы со список разадч
        /// </summary>
        public string UriList { get; set; }

        /// <summary>
        /// Выражение XPath для парсинга списка раздач
        /// </summary>
        public string XPathExprList { get; set; }

        /// <summary>
        /// Выражение XPath для парсинга даты добавления отдельно взятой раздачи
        /// </summary>
        public string XPathExprItemDate { get; set; }

        /// <summary>
        /// Выражение XPath для парсинга уникального номера отдельно взятой раздачи
        /// </summary>
        public string XPathExprItemUniqNumber { get; set; }

        /// <summary>
        /// Выражение XPath для парсинга названия отдельно взятой раздачи
        /// </summary>
        public string XPathExprItemName { get; set; }

        /// <summary>
        /// Адрес тор прокси
        /// </summary>
        public string ProxySocks5Addr { get; set; }

        /// <summary>
        /// Порт тор прокси
        /// </summary>
        public int ProxySocks5Port { get; set; }
    }
}
