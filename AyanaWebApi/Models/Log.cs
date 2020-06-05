using System;

namespace AyanaWebApi.Models
{
    /// <summary>
    /// Лог
    /// </summary>
    public class Log
    {
        public int Id { get; set; }

        /// <summary>
        /// Дата и время ошибки
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Место возникновения ошибки
        /// </summary>
        public string Location { get; set; }
    }
}
