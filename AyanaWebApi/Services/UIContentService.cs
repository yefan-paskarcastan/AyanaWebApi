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
        /// Запуск пакетного парсинга Rutor
        /// </summary>
        /// <param name="dayFromError"></param>
        /// <returns></returns>
        async Task<bool> ManagerRutor(int dayFromError)
        {
            IList<RutorListItem> errorList =
                _context
                .RutorListItems
                .FromSqlInterpolated($"RutorListItemsError {DateTime.Now.AddDays(-dayFromError)}")
                .ToList();

            if (errorList.Count > 0)
            {
                bool resTails = await FlowRutor(errorList);
                if (!resTails)
                {
                    _logger.LogError("Ошибка пакетной публикации хвостов");
                    return false;
                }
            }

            RutorCheckListInput rutorCheckListInput =
                _context
                .RutorCheckListInputs
                .Single(el => el.Active);
            IList<RutorListItem> rutorListItem = await _rutorService.CheckList(rutorCheckListInput);
            if (rutorListItem == null)
            {
                _logger.LogError("Ошибка проверки новых презентаций. RutorCheckListInput.Id = " + rutorCheckListInput.Id);
                return false;
            }

            bool res = await FlowRutor(rutorListItem);
            if (!res)
            {
                _logger.LogError("Ошибка пакетной публикации рогов");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Запуск пакетного парсинаг Nnmclub
        /// </summary>
        /// <returns></returns>
        async Task<bool> ManagerNnmclub(int dayFromError)
        {
            IList<NnmclubListItem> errorList =
                _context
                .NnmclubListItems
                .FromSqlInterpolated($"NnmclubListItemsError {DateTime.Now.AddDays(-dayFromError)}")
                .ToList();

            if (errorList.Count > 0)
            {
                bool resTails = await FlowNnmclub(errorList);
                if (!resTails)
                {
                    _logger.LogError("Ошибка пакетной публикации хвостов");
                    return false;
                }
            }

            NnmclubCheckListInput inp =
                _context
                .NnmclubCheckListInputs
                .Single(el => el.Active);
            IList<NnmclubListItem> list = await _nnmclubService.CheckList(inp);
            if (list == null)
            {
                _logger.LogError("Ошибка проверки новых презентаций. NnmclubCheckListInput.Id = " + inp.Id);
                return false;
            }
            bool res = await FlowNnmclub(list);
            if (!res)
            {
                _logger.LogError("Ошибка пакетной публикации рогов");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Проводит операции с полученным списком презентаций по публикации на сайте soft
        /// </summary>
        /// <param name="lst">Список презентаций, которые были получены с rutor</param>
        /// <returns>Если операция выполнена успешно - true, инчае false</returns>
        async Task<bool> FlowRutor(IList<RutorListItem> lst)
        {
            foreach (RutorListItem item in lst)
            {
                //Парсим
                RutorParseItemInput paramRutorItem =
                    _context.RutorParseItemInputs
                    .Single(el => el.Active);
                paramRutorItem.ListItemId = item.Id;
                RutorItem rutorItem = await _rutorService.ParseItem(paramRutorItem);
                if (rutorItem == null)
                {
                    _logger.LogError("Не удалось распарсить пост. ListItemId = " + item.Id);
                    return false;
                }

                //Выкладываем
                PublishResult result = await Send(nameof(RutorItem), rutorItem.Id);
                if (result == PublishResult.Error)
                {
                    _logger.LogError("Ошибка при отправке поста");
                    return false;
                }
                if (result == PublishResult.FileExist)
                {
                    _logger.LogError("Пост уже существует, переходим к следующему");
                    continue;
                }
            }
            return true;
        }

        /// <summary>
        /// Проводит операции с полученным списком презентаций по публикации на сайте soft
        /// </summary>
        /// <param name="lst">Список презентаций, которые были получены с nnmclub</param>
        /// <returns>Если все презентации выложены успешно, то true, инчае false</returns>
        async Task<bool> FlowNnmclub(IList<NnmclubListItem> lst)
        {
            foreach (NnmclubListItem item in lst)
            {
                //Парсим
                NnmclubParseItemInput paramParseInp =
                    _context.NnmclubParseItemInputs
                    .Single(el => el.Active);
                paramParseInp.ListItemId = item.Id;
                NnmclubItem nnmclubItem = await _nnmclubService.ParseItem(paramParseInp);
                if (nnmclubItem == null)
                {
                    _logger.LogError("Не удалось распарсить пост. NnmclubListItem.Id = " + item.Id);
                    return false;
                }

                //Выкладываем
                PublishResult result = await Send(nameof(NnmclubItem), nnmclubItem.Id);
                if (result == PublishResult.Error)
                {
                    _logger.LogError("Ошибка при отправке поста");
                    return false;
                }
                if (result == PublishResult.FileExist)
                {
                    _logger.LogError("Пост уже существует, переходим к следующему");
                    continue;
                }
            }
            return true;
        }

        /// <summary>
        /// Подготавливаем и выкладываем пост
        /// </summary>
        /// <param name="itemType">Тип поста</param>
        /// <param name="itemId">Id поста</param>
        /// <returns>Если метод выполнен успешно - true, инчае false</returns>
        async Task<PublishResult> Send(string itemType, int itemId)
        {
            //Подготавливаем
            DriverToSoftInput driverInput =
                _context.DriverToSoftInputs
                .Single(el => el.Active && el.Type == itemType);
            driverInput.ParseItemId = itemId;
            SoftPost post = await _driverService.Convert(driverInput);
            if (post == null)
            {
                _logger.LogError($"Не удалось подготовить пост к публикации. {itemType}.Id = " + itemId);
                return PublishResult.Error;
            }

            //Выкладываем
            SoftPostInput softPostInput =
                _context.SoftPostInputs
                .Single(el => el.Active);
            softPostInput.SoftPostId = post.Id;
            softPostInput.PosterUploadQueryString =
                _context.DictionaryValues
                .Where(el => el.DictionaryName == softPostInput.PosterUploadQueryStringId)
                .ToDictionary(k => k.Key, v => v.Value);
            softPostInput.TorrentUploadQueryString =
                _context.DictionaryValues
                .Where(el => el.DictionaryName == softPostInput.TorrentUploadQueryStringId)
                .ToDictionary(k => k.Key, v => v.Value);
            softPostInput.FormData =
                _context.DictionaryValues
                .Where(el => el.DictionaryName == softPostInput.FormDataId)
                .ToDictionary(k => k.Key, v => v.Value);
            softPostInput.AuthData =
                _context.DictionaryValues
                .Where(el => el.DictionaryName == softPostInput.AuthDataId)
                .ToDictionary(k => k.Key, v => v.Value);
            PublishResult result = await _softService.AddPost(softPostInput);
            if (result == PublishResult.Error)
            {
                _logger.LogError("Не удалось выложить пост на сайт. SoftPost.Id = " + post.Id);
                return PublishResult.Error;
            }
            if (result == PublishResult.FileExist)
            {
                _logger.LogError("Не удалось выложить пост на сайт. Такой файл уже загружен.");
                return PublishResult.FileExist;
            }
            return PublishResult.Success;
        }
    }
}
