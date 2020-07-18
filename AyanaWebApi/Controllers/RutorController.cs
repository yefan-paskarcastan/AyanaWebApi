using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;
using AyanaWebApi.Utils;

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
            ServiceResult<RutorItem> item = await _rutorService.ParseItem(parseParam);
            if (item.ResultObj != null)
            {

                foreach (var img in item.ResultObj.Imgs)
                {
                    img.RutorItem = null;
                }
                foreach (var spl in item.ResultObj.Spoilers)
                {
                    spl.RutorItem = null;
                }
                item.ResultObj.RutorListItem = null;
                return Ok(item);
            }
            return BadRequest("Не удалось распарсить");
        }

        [HttpPost("CheckList")]
        public async Task<ActionResult<IList<RutorListItem>>> CheckList([FromBody]RutorCheckListInput rutorCheckList)
        {
            ServiceResult<IList<RutorListItem>> result = await _rutorService.CheckList(rutorCheckList);

            if (result.ResultObj != null)
            {
                return Ok(result.ResultObj);
            }
            return BadRequest("При проверке списка раздач произошла ошибка");
        }

        readonly IRutorService _rutorService;
    }
}