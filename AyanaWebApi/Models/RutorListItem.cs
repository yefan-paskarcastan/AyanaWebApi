using System;
using System.Collections.Generic;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Экзепляр раздачи из списка раздач
    /// </summary>
    public class RutorListItem
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// Дата добавления из колонки "Добавлено"
        /// </summary>
        public DateTime AddedDate { get; set; }

        /// <summary>
        /// Уникальный номер раздачи
        /// </summary>
        public string HrefNumber { get; set; }

        /// <summary>
        /// Название раздачи, которое отображается пользователю
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Посты, полученные по этому экземпляру
        /// </summary>
        public List<RutorItem> RutorItems { get; set; }
    }
}
