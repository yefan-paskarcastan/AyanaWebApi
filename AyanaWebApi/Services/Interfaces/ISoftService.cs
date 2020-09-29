using System.Threading.Tasks;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services.Interfaces
{
    public interface ISoftService
    {
        /// <summary>
        /// Добавляет пост
        /// </summary>
        /// <param name="inputParam"></param>
        /// <returns></returns>
        Task<ServiceResult<SoftResult>> AddPost(SoftPostInput inputParam);
    }
}
