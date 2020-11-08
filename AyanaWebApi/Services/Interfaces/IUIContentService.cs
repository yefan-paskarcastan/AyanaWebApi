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

        IList<ListItem> GetProgramsList();
    }
}
