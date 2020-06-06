namespace AyanaWebApi.Models
{
    /// <summary>
    /// Парамерты для проверки списка разадач
    /// </summary>
    public class RutorCheckListInput
    {
        /// <summary>
        /// Адрес страницы со список разадч
        /// </summary>
        public string UriList { get; set; }

        /// <summary>
        /// Выражение XPath для парсинга даты добавления отдельно взятой раздачи
        /// </summary>
        public string XPathExprItemDate { get; set; }

        /// <summary>
        /// Выражение XPath для парсинга уникального номера отдельно взятой раздачи
        /// </summary>
        public string XPathExprItemUniqNumber { get; set; }

        /// <summary>
        /// Разделитель строки для парсинга уникального номера
        /// </summary>
        public string XPathParamSplitSeparator { get; set; }

        /// <summary>
        /// Индекс уникального номера в строке с разделителем
        /// </summary>
        public int XPathParamSplitIndex { get; set; }

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
