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

        /// <summary>
        /// Парсит указанную раздачу
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<RutorItem> ParseItemTest(RutorParseItemInput param);

        /// <summary>
        /// Парсит указанную раздачу и записывает ее в базу
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<RutorItem> ParseItem(RutorParseItemInput param);
    }
}
