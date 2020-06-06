using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AyanaWebApi.Models;

namespace AyanaWebApi.Services
{
    public interface IDriverService
    {
        Task<string> RutorTorrent(DriverRutorTorrentInput param);
    }
}
