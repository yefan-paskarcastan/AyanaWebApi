using System.Threading.Tasks;
using System.Collections.Generic;

using AyanaWebApi.Models;

namespace AyanaWebApi.Services
{
    public interface IRutorService
    {
        /// <summary>
        /// Проверит список раздач рутора и записать в базу
        /// </summary>
        /// <param name="rutorCheckList">Ссылка на список рутора</param>
        /// <returns></returns>
        Task<IList<RutorListItem>> CheckList(RutorCheckListInput rutorCheckList);

        /// <summary>
        /// Возвращает список найденных раздач на странице со списком
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IList<RutorListItem>> CheckListTest(RutorCheckListInput param);

        Task<RutorItem> ParseItem(RutorParseItemInput param);
    }
}
