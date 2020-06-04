using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AyanaWebApi.ApiEntities;

namespace AyanaWebApi.Utils
{
    public interface ITorrentSoftService
    {
        /// <summary>
        /// Добавляет тестовый пост. Проверка работоспособности
        /// </summary>
        /// <param name="inputParam"></param>
        /// <returns></returns>
        Task<bool> AddPostTest(TorrentSoftAddPostTestInput inputParam);
    }
}
