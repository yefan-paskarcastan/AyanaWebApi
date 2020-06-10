using System.Threading.Tasks;
using AyanaWebApi.Models;

namespace AyanaWebApi.Services.Interfaces
{
    public interface ITorrentSoftService
    {
        /// <summary>
        /// Добавляет тестовый пост. Проверка работоспособности
        /// </summary>
        /// <param name="inputParam"></param>
        /// <returns></returns>
        Task<bool> AddPostTest(TorrentSoftAddPostTestInput inputParam);
    }
}
