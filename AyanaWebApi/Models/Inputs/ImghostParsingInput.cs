using System;

namespace AyanaWebApi.Models
{
    public class ImghostParsingInput
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// Строка по которой определяется принадлежность
        /// </summary>
        public string Def { get; set; }

        /// <summary>
        /// Выражение для поиска скриншота
        /// </summary>
        public string XPath { get; set; }

        /// <summary>
        /// Название аттрибута для парсинга
        /// </summary>
        public string Attr { get; set; }

        /// <summary>
        /// Действующая настройка
        /// </summary>
        public bool Active { get; set; }
    }
}
