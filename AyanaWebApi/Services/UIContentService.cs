using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Internal;

using BencodeNET.Objects;
using BencodeNET.Parsing;
using BencodeNET.Torrents;

using AyanaWebApi.Utils;
using AyanaWebApi.DTO;
using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Services
{
    public class UIContentService : IUIContentService
    {
        readonly AyDbContext _context;
        readonly ILogger<EveningWorkService> _logger;
        readonly IRutorService _rutorService;
        readonly INnmclubService _nnmclubService;
        readonly IDriverService _driverService;
        readonly ISoftService _softService;

        public UIContentService(AyDbContext ayDbContext,
                                  IRutorService rutorService,
                                  INnmclubService nnmclubService,
                                  IDriverService driverService,
                                  ISoftService softService,
                                  ILogger<EveningWorkService> logger)
        {
            _context = ayDbContext;
            _rutorService = rutorService;
            _nnmclubService = nnmclubService;
            _driverService = driverService;
            _softService = softService;
            _logger = logger;
        }

        public bool Prepare()
        {
            var query =
                from el in _context.SoftPosts
                where el.Id == 85 || el.Id == 132 || el.Id == 311
                select el;

            IList<SoftPost> lst = query.ToList();

            var parser = new BencodeParser();
            Torrent file0 = parser.Parse<Torrent>(lst[0].TorrentFile);
            Torrent file1 = parser.Parse<Torrent>(lst[1].TorrentFile);
            Torrent file2 = parser.Parse<Torrent>(lst[2].TorrentFile);

            Torrent softFirefox = parser.Parse<Torrent>(@"F:\VS Projects\[Torrent-Soft.Net]_Firefox Browser 81.0.1.torrent");
            Torrent nnmFirefox = parser.Parse<Torrent>(@"F:\VS Projects\nnm club firefox.torrent");
            Torrent rutorFirefox = parser.Parse<Torrent>(@"F:\VS Projects\rutor firefox.torrent");

            var trackers = softFirefox.Trackers.Union(rutorFirefox.Trackers);

            softFirefox.Trackers = trackers.ToList();
            softFirefox.EncodeTo(@"F:\VS Projects\[Custom]rutor and soft firefox.torrent");

            return true;
        }

        /// <summary>
        /// Возвращает список программ для отображения
        /// </summary>
        /// <returns></returns>
        public IList<ListItem> GetProgramsList(Pagination pagination)
        {
            var query =
                from el in _context.NnmclubItems
                where el.Actual == true
                orderby el.Created descending
                select new ListItem
                {
                    Created = el.Created,
                    Name = el.Name
                };
            IList<ListItem> lst = query
                .Page(pagination.CurrentPage, pagination.CountItem)
                .ToList();
            return lst;
        }
    }
}
