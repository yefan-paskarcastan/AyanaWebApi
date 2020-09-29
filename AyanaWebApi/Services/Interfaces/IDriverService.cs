using System.Threading.Tasks;

using AyanaWebApi.Models;

namespace AyanaWebApi.Services.Interfaces
{
    public interface IDriverService
    {
        /// <summary>
        /// Получает пост и сохраняет его в базу
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<TorrentSoftPost> Convert(DriverRutorTorrentInput param);
    }
}
