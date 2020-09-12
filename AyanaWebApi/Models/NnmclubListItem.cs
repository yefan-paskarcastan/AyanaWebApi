using System;
using System.Collections.Generic;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Экземпляр презентации из списка клуба
    /// </summary>
    public class NnmclubListItem
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// Дата добавления в клуб
        /// </summary>
        public DateTime Added { get; set; }

        /// <summary>
        /// Название презентации
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Адрес презентации
        /// </summary>
        public string Href { get; set; }
    }
}
