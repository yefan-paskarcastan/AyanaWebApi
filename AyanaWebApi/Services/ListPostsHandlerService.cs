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

        public async Task<string> Publishing()
        {
            RutorCheckListInput rutorCheckListInput =
                _context
                .RutorCheckListInputs
                .Single(el => el.Active);
            ServiceResult<IList<RutorListItem>> rutorListItem = await _rutorService.CheckList(rutorCheckListInput);
            if (rutorListItem.ResultObj == null)
                return "Не удалось проверить список раздач rutor. RutorCheckListInputId = " + rutorCheckListInput.Id;

            foreach (RutorListItem item in rutorListItem.ResultObj)
            {
                RutorParseItemInput paramRutorItem = 
                    _context
                    .RutorParseItemInputs
                    .Single(el => el.Active);
                paramRutorItem.ListItemId = item.Id;
                ServiceResult<RutorItem> rutorItem = await _rutorService.ParseItem(paramRutorItem);
                if (rutorItem.ResultObj == null)
                    return "Не удалось распарсить пост RutorItem. ListItemId = " + item.Id;

                DriverRutorTorrentInput driverTorrentInput =
                    _context
                    .DriverRutorTorrentInputs
                    .Single(el => el.Active);
                driverTorrentInput.ParseItemId = rutorItem.ResultObj.Id;
                TorrentSoftPost post = await _driverService.RutorTorrent(driverTorrentInput);
                if (post == null)
                    return "Не удалось подготовить пост к публикации. RutorItemId = " + rutorItem.ResultObj.Id;

                TorrentSoftPostInput torrentSoftPostInput =
                    _context
                    .TorrentSoftPostInputs
                    .Single(el => el.Active);
                torrentSoftPostInput.TorrentSoftPostId = post.Id;
                ServiceResult<TorrentSoftResult> result = await _torrentSoftService.AddPost(torrentSoftPostInput);
                if (result.ResultObj.TorrentSoftPost == null
                    || !result.ResultObj.SendPostIsSuccess
                    || !result.ResultObj.PosterIsSuccess
                    || !result.ResultObj.TorrentFileIsSuccess)
                    return "Не удалось выложить пост на сайт. TorrentSoftPostId = " + post.Id;
            }
            return "Посты успешно добавлены.";
        }

        AyDbContext _context;
        IRutorService _rutorService;
        IDriverService _driverService;
        ITorrentSoftService _torrentSoftService;
    }
}
