using System.Threading.Tasks;
using AyanaWebApi.Models;
using AyanaWebApi.ApiEntities;

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
    }
}
