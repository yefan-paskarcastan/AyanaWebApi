using System;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Результат парсинга
    /// </summary>
    public class ParseResult
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        /// <summary>
        /// Сообщение о результате парсинга
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Результат парсинга. Если истина, значит все прошло без ошибок
        /// </summary>
        public bool Success { get; set; }
    }
}
