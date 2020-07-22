using System.Threading.Tasks;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services.Interfaces
{
    public interface ITorrentSoftService
    {
        /// <summary>
        /// Добавляет пост
        /// </summary>
        /// <param name="inputParam"></param>
        /// <returns></returns>
        Task<ServiceResult<TorrentSoftResult>> AddPost(TorrentSoftPostInput inputParam);
    }
}
