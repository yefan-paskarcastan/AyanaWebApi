using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Services
{
    public class LogService
    {
        public LogService(AyDbContext ayDbContext)
        {
            _context = ayDbContext;
        }

        public void Write(IServiceResult serviceResult)
        {

        }

        AyDbContext _context;
    }
}
