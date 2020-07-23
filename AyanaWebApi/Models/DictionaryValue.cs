namespace AyanaWebApi.Models
{
    public class DictionaryValue
    {
        public int Id { get; set; }

        /// <summary>
        /// Название словаря
        /// </summary>
        public string DictionaryName { get; set; }

        /// <summary>
        /// Ключ
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
    }
}
