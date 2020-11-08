using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.DTO
{
    public class Pagination
    {
        /// <summary>
        /// Всего страниц
        /// </summary>
        public int CountPage { get; set; }

        /// <summary>
        /// Текущая страница
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Количество объектов на странице
        /// </summary>
        public int CountItem { get; set; }
    }
}
