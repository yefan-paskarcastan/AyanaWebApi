using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Services
{
    public class TegreiUIService : ITegreiUIService
    {
        public TegreiUIService(AyDbContext ayDbContext)
        {
            _context = ayDbContext;
        }

        /// <summary>
        /// Возвращает список разадч Rutor для UI
        /// </summary>
        /// <returns></returns>
        public async Task<IList<RutorItem>> RutorList()
        {
            IList<RutorItem> lst =  await _context
                .RutorItems
                .Include(el => el.RutorListItem)
                .Include(el => el.Imgs)
                .Include(el => el.Spoilers)
                .OrderByDescending(el => el.Created)
                .Take(100)
                .ToListAsync();
            return lst;
        }

        AyDbContext _context;
    }
}
