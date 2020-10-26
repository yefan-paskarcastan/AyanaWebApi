using System.Threading.Tasks;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services.Interfaces
{
    public interface ISoftService
    {
        /// <summary>
        /// Добавляет пост на сайт
        /// </summary>
        /// <param name="inputParam"></param>
        /// <returns>Если отработал успешно, то true, инчае false</returns>
        Task<bool> AddPost(SoftPostInput inputParam);
    }
}
