using System.Threading.Tasks;
using System.Collections.Generic;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services.Interfaces
{
    public interface IRutorService
    {
        /// <summary>
        /// Проверит список раздач рутора и записать в базу
        /// </summary>
        /// <param name="rutorCheckList">Ссылка на список рутора</param>
        /// <returns></returns>
        Task<ServiceResult<IList<RutorListItem>>> CheckList(RutorCheckListInput rutorCheckList);

        /// <summary>
        /// Парсит указанную раздачу и записывает ее в базу
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<ServiceResult<RutorItem>> ParseItem(RutorParseItemInput param);
    }
}
