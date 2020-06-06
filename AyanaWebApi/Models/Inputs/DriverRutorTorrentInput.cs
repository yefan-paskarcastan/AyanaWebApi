using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    public class DriverRutorTorrentInput
    {
        /// <summary>
        /// Id полученной с сайта раздачи
        /// </summary>
        public int ParseItemId { get; set; }

        /// <summary>
        /// Шаблон адреса для загрузки торрент файла
        /// </summary>
        public string TorrentUri { get; set; }

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
