using System;
using System.Collections.Generic;

namespace AyanaWebApi.Models
{
    public class NnmclubItem
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// Флаг, указывающий, что презентация является самой последней в раздаче на текущий момент
        /// </summary>
        public bool Actual { get; set; }

        /// <summary>
        /// Ссылка на раздачу
        /// </summary>
        public string GroupHref { get; set; }

        /// <summary>
        /// Заголовок презентации
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание презентации
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Спойлеры презентации
        /// </summary>
        public List<NnmclubItemSpoiler> Spoilers { get; set; }

        /// <summary>
        /// Скриншоты презентации
        /// </summary>
        public List<NnmclubItemImg> Imgs { get; set; }

        /// <summary>
        /// Постер презентации
        /// </summary>
        public string Poster { get; set; }

        /// <summary>
        /// Ссылка на торрент файл
        /// </summary>
        public string Torrent { get; set; }

        public int NnmclubListItemId { get; set; }

        public NnmclubListItem NnmclubListItem { get; set; }
    }
}
