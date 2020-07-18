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
        /// Сообщение от исключения
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Содержимое объекта ошикби
        /// </summary>
        public string ErrorContent { get; set; }

        /// <summary>
        /// Имя класса, где создавался результат
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Имя метода, где создавался результат
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Место возникновения ошибки
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Стек трейс, если возник эксепшен
        /// </summary>
        public string StackTrace { get; set; }
    }
}
