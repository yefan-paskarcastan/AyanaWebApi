using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Небольшая текстовая информация, представленная на отдельной странице
    /// </summary>
    public class TegreiArticle
    {
        /// <summary>
        /// Название статьи в базе
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Заголовок статьи
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Текст статьи
        /// </summary>
        public string Text { get; set; }
    }
}
