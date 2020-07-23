using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AyanaWebApi.Models;

namespace AyanaWebApi.Services.Interfaces
{
    public interface IListPostsHandlerService
    {
        Task<string> Publishing();
    }
}
