namespace AyanaWebApi.Models
{
    public class ImghostParamsParsing
    {
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
    }
}
