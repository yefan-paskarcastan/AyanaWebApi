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

        readonly AyDbContext _context;
        readonly ILogger<EveningWorkService> _logger;
        readonly IRutorService _rutorService;
        readonly INnmclubService _nnmclubService;
        readonly IDriverService _driverService;
        readonly ISoftService _softService;

        public async Task<bool> Publishing(int dayFromError)
        {
            bool resultNnmclub = await ManagerNnmclub(dayFromError);
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
                _logger.LogError("Ошибка проверки новых презентаций. RutorCheckListInput.Id = " + rutorCheckListInput.Id);
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
                ServiceResult<RutorItem> rutorItem = await _rutorService.ParseItem(paramRutorItem);
                if (rutorItem.ResultObj == null)
                {
                    _logger.LogError("Не удалось распарсить пост. ListItemId = " + item.Id);
                    return false;
                }

                //Выкладываем
                bool result = await Send(nameof(RutorItem), rutorItem.ResultObj.Id);
                if (!result)
                {
                    _logger.LogError("Ошибка при отправке поста");
                    return false;
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
                bool result = await Send(nameof(NnmclubItem), nnmclubItem.Id);
                if (!result)
                {
                    _logger.LogError("Ошибка при отправке поста");
                    return false;
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
        async Task<bool> Send(string itemType, int itemId)
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
            bool result = await _softService.AddPost(softPostInput);
            if (!result)
            {
                _logger.LogError("Не удалось выложить пост на сайт. SoftPost.Id = " + post.Id);
                return false;
            }
            return true;
        }
    }
}
