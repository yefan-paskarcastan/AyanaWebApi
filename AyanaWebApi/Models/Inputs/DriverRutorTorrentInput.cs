﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AyanaWebApi.Models
{
    public class DriverRutorTorrentInput
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public bool Active { get; set; }

        /// <summary>
        /// Id полученной с сайта раздачи
        /// </summary>
        [NotMapped]
        public int ParseItemId { get; set; }

        /// <summary>
        /// Шаблон адреса для загрузки торрент файла
        /// </summary>
        public string TorrentUri { get; set; }

        /// <summary>
        /// Максимальный размер постера
        /// </summary>
        public int MaxPosterSize { get; set; }

        /// <summary>
        /// Адрес тор прокси
        /// </summary>
        public string ProxySocks5Addr { get; set; }

        /// <summary>
        /// Порт тор прокси
        /// </summary>
        public int ProxySocks5Port { get; set; }

        /// <summary>
        /// Флаг использования тор проски
        /// </summary>
        public bool ProxyActive { get; set; }
    }
}
