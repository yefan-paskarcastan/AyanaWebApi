using AyanaWebApi.Utils;

namespace AyanaWebApi.Services.Interfaces
{
    public interface ILogService
    {
        /// <summary>
        /// Записать в лог
        /// </summary>
        /// <param name="serviceResult"></param>
        void Write(IServiceResult serviceResult);
    }
}
