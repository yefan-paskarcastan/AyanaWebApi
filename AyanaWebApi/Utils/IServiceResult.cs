using System;

namespace AyanaWebApi.Utils
{
    public interface IServiceResult
    {
        /// <summary>
        /// Комментарий к результату
        /// </summary>
        string Comment { get; set; }

        /// <summary>
        /// Сообщение от исключения
        /// </summary>
        string ExceptionMessage { get; set; }

        /// <summary>
        /// Содержимое объекта ошикби
        /// </summary>
        string ErrorContent { get; set; }

        /// <summary>
        /// Время создания результата
        /// </summary>
        DateTime Created { get; }

        /// <summary>
        /// Имя класса, где создавался результат
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// Имя метода, где создавался результат
        /// </summary>
        string MethodName { get; }

        /// <summary>
        /// Где создан результат выполнения сервиса на простом языке
        /// </summary>
        public string Location { get; set; }
    }
}
