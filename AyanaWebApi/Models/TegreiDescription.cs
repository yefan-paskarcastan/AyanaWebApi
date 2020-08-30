using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Основная информация о сайте
    /// </summary>
    public class TegreiDescription
    {
        /// <summary>
        /// Название сайта, которое будет отображаться в заголовке и подвале
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Контактный email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Краткий дисклеймер
        /// </summary>
        public string Description { get; set; }
    }
}
