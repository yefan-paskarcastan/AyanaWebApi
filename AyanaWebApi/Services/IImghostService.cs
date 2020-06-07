using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AyanaWebApi.Models;

namespace AyanaWebApi.Services
{
    public interface IImghostService
    {
        /// <summary>
        /// Возвращает прямые ссылки для указанных изображений
        /// </summary>
        /// <param name="listImg"></param>
        /// <returns></returns>
        Task<IList<string>> GetOriginalsUri(ImghostGetOriginalsInput param);
    }
}
