using System.Threading.Tasks;

using AyanaWebApi.Models;

namespace AyanaWebApi.Services.Interfaces
{
    public interface IDriverService
    {
        Task<TorrentSoftPost> RutorTorrent(DriverRutorTorrentInput param);

        Task<TorrentSoftPost> RutorTorrentTest(DriverRutorTorrentInput param);
    }
}
