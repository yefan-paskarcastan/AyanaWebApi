using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AyanaWebApi.DTO;

namespace AyanaWebApi.Services.Interfaces
{
    public interface IUIContentService
    {
        bool Prepare();

        /// <summary>
        /// Возвращает список программ для отображения
        /// </summary>
        /// <returns></returns>
        IList<ListItem> GetProgramsList(Pagination pagination);
    }
}
