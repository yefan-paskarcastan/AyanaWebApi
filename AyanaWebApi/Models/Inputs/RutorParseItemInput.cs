using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Найстройки для парсинга поста
    /// </summary>
    public class RutorParseItemInput
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
        /// XPath выражение для парсинга описания
        /// </summary>
        public string XPathExprDescription { get; set; }

        /// <summary>
        /// XPath выражение для парсинга спойлеров
        /// </summary>
        public string XPathExprSpoiler { get; set; }

        /// <summary>
        /// XPath выражения для парсинга картинок
        /// </summary>
        public string XPathExprImgs { get; set; }

        /// <summary>
        /// Id объекта листа, по которму будет строится адрес страницы
        /// </summary>
        [NotMapped]
        public int ListItemId { get; set; }
    }
}
