using System.Threading.Tasks;
using AyanaWebApi.Models;
using AyanaWebApi.ApiEntities;
using System.Collections.Generic;

namespace AyanaWebApi.Utils
{
    public interface IRutorService
    {
        /// <summary>
        /// Проверит список раздач рутора и записать в базу
        /// </summary>
        /// <param name="rutorCheckList">Ссылка на список рутора</param>
        /// <returns></returns>
        Task<ParseResult> CheckList(RutorCheckList rutorCheckList);

        /// <summary>
        /// Возвращает список найденных раздач на странице со списком
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IList<RutorListItem>> CheckListSettings(RutorCheckList param);

        Task<RutorItem> ParseItem(RutorInputParseItem param);
    }
}
