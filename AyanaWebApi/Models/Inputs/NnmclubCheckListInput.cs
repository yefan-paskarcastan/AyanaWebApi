﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Параметры для проверки списка презентаций клуба
    /// </summary>
    public class NnmclubCheckListInput
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// Флаг выбарнной настройки
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Адерс страницы со списком презентаций
        /// </summary>
        public string UriList { get; set; }

        /// <summary>
        /// Выражение XPath для парсинга даты добавления презентации
        /// </summary>
        public string XPathDate { get; set; }

        /// <summary>
        /// Адрес тор прокси
        /// </summary>
        public string ProxySocks5Addr { get; set; }

        /// <summary>
        /// Порт тор прокси
        /// </summary>
        public int ProxySocks5Port { get; set; }

        /// <summary>
        /// Флаг использования тор прокси
        /// </summary>
        public bool ProxyActive { get; set; }
    }
}
