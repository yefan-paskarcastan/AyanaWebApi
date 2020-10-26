using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services
{
    public class EveningWorkService : IEveningWorkService
    {
        public EveningWorkService(AyDbContext ayDbContext,
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

        public async Task<bool> Publishing(int dayFromError)
        {
            bool resultNnmclub = await ManagerNnmclub();
            bool resultRutor = await ManagerRutor(dayFromError);
            return resultRutor && resultNnmclub;
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
            ServiceResult<IList<RutorListItem>> rutorListItem = await _rutorService.CheckList(rutorCheckListInput);
            if (rutorListItem.ResultObj == null)
            {
                _logger.LogError("Ошибка проверки новых презентаций. RutorCheckListInput_Id = " + rutorCheckListInput.Id);
                return false;
            }

            bool res = await FlowRutor(rutorListItem.ResultObj);
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
        async Task<bool> ManagerNnmclub()
        {
            NnmclubCheckListInput inp =
                _context
                .NnmclubCheckListInputs
                .Single(el => el.Active);
            IList<NnmclubListItem> list = await _nnmclubService.CheckList(inp);
            if (list == null)
            {
                _logger.LogError("Ошибка проверки новых презентаций. NnmclubCheckListInput_Id = " + inp.Id);
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
                ServiceResult<RutorItem> rutorItem = await _rutorService.ParseItem(paramRutorItem);
                if (rutorItem.ResultObj == null)
                {
                    _logger.LogError("Не удалось распарсить пост. ListItemId = " + item.Id);
                    return false;
                }

                //Подготавливаем
                DriverToSoftInput driverInput =
                    _context.DriverToSoftInputs
                    .Single(el => el.Active && el.Type == nameof(RutorItem));
                driverInput.ParseItemId = rutorItem.ResultObj.Id;
                SoftPost post = await _driverService.Convert(driverInput);
                if (post == null)
                {
                    _logger.LogError("Не удалось подготовить пост к публикации. RutorItemId = " + rutorItem.ResultObj.Id);
                    return false;
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
                ServiceResult<SoftResult> result = await _softService.AddPost(softPostInput);
                if (result.ResultObj.SoftPost == null
                    || !result.ResultObj.SendPostIsSuccess
                    || !result.ResultObj.PosterIsSuccess
                    || !result.ResultObj.TorrentFileIsSuccess)
                {
                    _logger.LogError("Не удалось выложить пост на сайт. TorrentSoftPostId = " + post.Id);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Проводит операции с полученным списком презентаций по публикации на сайте soft
        /// </summary>
        /// <param name="lst">Список презентаций, которые были получены с nnmclub</param>
        /// <returns>Если все презентации выложены успешно, то ResultObj != null</returns>
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
                    _logger.LogError("Не удалось распарсить пост. ListItemId = " + item.Id);
                    return false;
                }

                //Подготавливаем
                DriverToSoftInput driverInput =
                    _context.DriverToSoftInputs
                    .Single(el => el.Active && el.Type == nameof(NnmclubItem));
                driverInput.ParseItemId = nnmclubItem.Id;
                SoftPost post = await _driverService.Convert(driverInput);
                if (post == null)
                {
                    _logger.LogError("Не удалось подготовить пост к публикации. RutorItemId = " + nnmclubItem.Id);
                    return false;
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
                ServiceResult<SoftResult> result = await _softService.AddPost(softPostInput);
                if (result.ResultObj.SoftPost == null
                    || !result.ResultObj.SendPostIsSuccess
                    || !result.ResultObj.PosterIsSuccess
                    || !result.ResultObj.TorrentFileIsSuccess)
                {
                    _logger.LogError("Не удалось выложить пост на сайт. TorrentSoftPostId = " + post.Id);
                    return false;
                }
            }
            return true;
        }

        readonly AyDbContext _context;
        readonly ILogger<EveningWorkService> _logger;
        readonly IRutorService _rutorService;
        readonly INnmclubService _nnmclubService;
        readonly IDriverService _driverService;
        readonly ISoftService _softService;
    }
}
