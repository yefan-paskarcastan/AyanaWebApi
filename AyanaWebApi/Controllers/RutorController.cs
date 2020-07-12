using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RutorController : ControllerBase
    {
        public RutorController(IRutorService rutorService)
        {
            _rutorService = rutorService;
        }

        [HttpPost("ParseItem")]
        public async Task<ActionResult<RutorItem>> ParseItem([FromBody] RutorParseItemInput parseParam)
        {
            RutorItem item = await _rutorService.ParseItem(parseParam);
            if (item != null)
            {
                foreach (var img in item.Imgs)
                {
                    img.RutorItem = null;
                }
                foreach (var spl in item.Spoilers)
                {
                    spl.RutorItem = null;
                }
                item.RutorListItem = null;
                return Ok(item);
            }
            return BadRequest("Не удалось распарсить");
        }

        [HttpPost("CheckList")]
        public async Task<ActionResult<IList<RutorListItem>>> CheckList([FromBody]RutorCheckListInput rutorCheckList)
        {
            var result = await _rutorService.CheckList(rutorCheckList);

            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("При проверке списка раздач произошла ошибка");
        }

        readonly IRutorService _rutorService;
    }
}