﻿using System.Collections.Generic;
using System.Threading.Tasks;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services.Interfaces
{
    public interface INnmclubService
    {
        /// <summary>
        /// Проверить список презентаций клуба
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IList<NnmclubListItem>> CheckList(NnmclubCheckListInput param);

        /// <summary>
        /// Парсит указанную презентацию
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<NnmclubItem> ParseItem(NnmclubParseItemInput param);
    }
}
