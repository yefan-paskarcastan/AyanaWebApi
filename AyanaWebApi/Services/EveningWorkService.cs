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
                                  IDriverService driverService,
                                  ISoftService softService,
                                  ILogService logService,
                                  ILogger<EveningWorkService> logger)
        {
            _context = ayDbContext;
            _rutorService = rutorService;
            _driverService = driverService;
            _softService = softService;
            _logService = logService;
            _logger = logger;
            logger.LogInformation("It's work!");
        }

        public async Task<string> Publishing(int dayFromError)
        {
            string date = DateTime.Now.AddDays(-dayFromError).ToShortDateString();
            IList<RutorListItem> errorList = 
                _context
                .RutorListItems
                .FromSqlInterpolated($"RutorListItemsError {date}")
                .ToList();

            if (errorList.Count > 0)
            {
                ServiceResult<string> resultError = await FlowRutor(errorList);
                if (resultError.ResultObj == null)
                {
                    _logService.Write(resultError);
                    return "Произошла ошибка при публикации хвостов";
                }
            }

            RutorCheckListInput rutorCheckListInput =
                _context
                .RutorCheckListInputs
                .Single(el => el.Active);
            ServiceResult<IList<RutorListItem>> rutorListItem = await _rutorService.CheckList(rutorCheckListInput);
            if (rutorListItem.ResultObj == null)
                return "Не удалось проверить список раздач rutor. RutorCheckListInputId = " + rutorCheckListInput.Id;

            ServiceResult<string> result = await FlowRutor(rutorListItem.ResultObj);
            if (result.ResultObj == null)
            {
                _logService.Write(result);
                return "Произошла ошибка при публикации презентаций";
            }

            return "Посты успешно добавлены.";
        }

        /// <summary>
        /// Проводит операции с полученным списком презентаций по публикации на сайте soft
        /// </summary>
        /// <param name="lst">Список презентаций, которые были получены с rutor</param>
        /// <returns>Если все презентации выложены успешно, то ResultObj != null</returns>
        async Task<ServiceResult<string>> FlowRutor(IList<RutorListItem> lst)
        {
            var serviceResult = new ServiceResult<string>
            {
                ServiceName = "ListPostsHandlerService",
                Location = "Пакетная публикация на сайт soft",
                ResultObj = "Success"
            };

            foreach (RutorListItem item in lst)
            {
                RutorParseItemInput paramRutorItem =
                    _context.RutorParseItemInputs
                    .Single(el => el.Active);
                paramRutorItem.ListItemId = item.Id;
                ServiceResult<RutorItem> rutorItem = await _rutorService.ParseItem(paramRutorItem);
                if (rutorItem.ResultObj == null)
                {
                    serviceResult.Comment = "Не удалось распарсить пост RutorItem. ListItemId = " + item.Id;
                    serviceResult.ResultObj = null;
                    break;
                }

                DriverToSoftInput driverInput =
                    _context.DriverToSoftInputs
                    .Single(el => el.Active && el.Type == nameof(RutorItem));
                driverInput.ParseItemId = rutorItem.ResultObj.Id;
                SoftPost post = await _driverService.Convert(driverInput);
                if (post == null)
                {
                    serviceResult.Comment = "Не удалось подготовить пост к публикации. RutorItemId = " + rutorItem.ResultObj.Id;
                    serviceResult.ResultObj = null;
                    break;
                }

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
                    serviceResult.Comment = "Не удалось выложить пост на сайт. TorrentSoftPostId = " + post.Id;
                    serviceResult.ResultObj = null;
                    break;
                }
            }
            return serviceResult;
        }

        /*/// <summary>
        /// Проводит операции с полученным списком презентаций по публикации на сайте soft
        /// </summary>
        /// <param name="lst">Список презентаций, которые были получены с nnmclub</param>
        /// <returns>Если все презентации выложены успешно, то ResultObj != null</returns>
        async Task<string> FlowNnmclub(IList<NnmclubListItem> lst)
        {

        }*/

        readonly AyDbContext _context;
        readonly ILogger<EveningWorkService> _logger;
        readonly IRutorService _rutorService;
        readonly IDriverService _driverService;
        readonly ISoftService _softService;
        readonly ILogService _logService;
    }
}
