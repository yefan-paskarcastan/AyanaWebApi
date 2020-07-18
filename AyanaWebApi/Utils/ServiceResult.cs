using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AyanaWebApi.Utils
{
    public class ServiceResult<T> : IServiceResult
                 where T : class
    {
        public ServiceResult([CallerMemberName]string methodName = "")
        {
            Created = DateTime.Now;
            MethodName = methodName;
            ResultObj = null;
        }

        /// <summary>
        /// Результат выполнения сервиса
        /// </summary>
        public T ResultObj { get; set; }

        /// <summary>
        /// Комментарий к результату
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Сообщение от исключения
        /// </summary>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Содержимое объекта ошикби
        /// </summary>
        public string ErrorContent { get; set; }

        /// <summary>
        /// Время создания результата
        /// </summary>
        public DateTime Created { get; }

        /// <summary>
        /// Имя класса, где создавался результат
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Имя метода, где создавался результат
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Где создан результат выполнения сервиса на простом языке
        /// </summary>
        public string Location { get; set; }
    }
}
