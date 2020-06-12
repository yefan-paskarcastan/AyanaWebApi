using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services
{
    public class ListPostsHandlerService : IListPostsHandlerService
    {
        public ListPostsHandlerService(AyDbContext ayDbContext,
                                       IRutorService rutorService,
                                       IDriverService driverService,
                                       ITorrentSoftService torrentSoftService)
        {
            _context = ayDbContext;
            _rutorService = rutorService;
            _driverService = driverService;
            _torrentSoftService = torrentSoftService;
        }

        public async Task<string> Published(TorrentSoftAddPostInput param)
        {
            foreach (int item in param.TorrentSoftPostList)
            {
                RutorParseItemInput paramRutorItem = _context
                        .RutorParseItemInputs
                        .SingleOrDefault(el => el.Active);
                paramRutorItem.ListItemId = item;
                RutorItem rutorItem = await _rutorService.ParseItem(paramRutorItem);
                if (rutorItem == null)
                    return "Не удалось распарсить пост RutorItem. ListItemId = " + item;

                DriverRutorTorrentInput driverTorrentInput = _context
                                                .DriverRutorTorrentInputs
                                                .SingleOrDefault(el => el.Active);
                driverTorrentInput.ParseItemId = rutorItem.Id;
                TorrentSoftPost post = await _driverService.RutorTorrent(driverTorrentInput);
                if (post == null)
                    return "Не удалось подготовить пост к публикации. RutorItemId = " + rutorItem.Id;

                param.TorrentSoftPostId = post.Id;
                TorrentSoftAddPostResult result = await _torrentSoftService.AddPostTest(param);
                if (result == TorrentSoftAddPostResult.Faild)
                    return "Не удалось выложить пост на сайт. TorrentSoftPostId = " + post.Id;
            }
            return "Посты успешно добавлены.";
        }

        #region Private
        AyDbContext _context;

        IRutorService _rutorService;

        IDriverService _driverService;

        ITorrentSoftService _torrentSoftService;

        #endregion
    }
}
