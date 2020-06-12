using System.Threading.Tasks;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services.Interfaces
{
    public interface ITorrentSoftService
    {
        /// <summary>
        /// Добавляет тестовый пост. Проверка работоспособности
        /// </summary>
        /// <param name="inputParam"></param>
        /// <returns></returns>
        Task<TorrentSoftAddPostResult> AddPostTest(TorrentSoftAddPostInput inputParam);
    }
}
