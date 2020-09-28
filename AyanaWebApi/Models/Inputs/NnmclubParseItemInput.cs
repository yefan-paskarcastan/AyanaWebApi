using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Настройки для парсинга презентации
    /// </summary>
    public class NnmclubParseItemInput
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// Действующая настройка
        /// </summary>
        public bool Active { get; set; }

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
        /// Флаг использования тор прокси, если истина
        /// </summary>
        public bool ProxyActive { get; set; }

        /// <summary>
        /// XPath выражение для парсинга описания
        /// </summary>
        public string XPathDescription { get; set; }

        /// <summary>
        /// XPath выражение для парсинга спойлеров
        /// </summary>
        public string XPathSpoiler { get; set; }

        /// <summary>
        /// XPath выражение для парсинга постера
        /// </summary>
        public string XPathPoster { get; set; }

        /// <summary>
        /// XPath выражения для парсинга картинок
        /// </summary>
        public string XPathImgs { get; set; }

        /// <summary>
        /// XPath выражения для удаления мусора из презентации
        /// </summary>
        public List<string> XPathTrash { get; set; }

        /// <summary>
        /// Id объекта листа, по которму будет строится адрес страницы
        /// </summary>
        [NotMapped]
        public int ListItemId { get; set; }
    }
}
