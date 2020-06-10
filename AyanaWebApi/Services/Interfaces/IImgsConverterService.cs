using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AyanaWebApi.Services.Interfaces
{
    public interface IImgsConverterService
    {
        string ConvertToJpg(string fullImgName, long quality);
    }
}
