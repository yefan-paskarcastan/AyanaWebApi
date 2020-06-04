using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AyanaWebApi.Models;
using AyanaWebApi.Utils;
using AyanaWebApi.ApiEntities;

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
        public async Task<ActionResult<RutorItem>> ParseItem([FromBody]RutorInputParseItem parseParam)
        {
            RutorItem item = await _rutorService.ParseItem(parseParam);
            if (item != null)
            {
                return Ok(item);
            }
            return BadRequest("Указанные id раздачи неверен или не удалось загрузить страницу");
        }

        [HttpPost("CheckListSettings")]
        public async Task<ActionResult<IList<RutorListItem>>> CheckListSettings([FromBody]RutorCheckList rutorCheckList)
        {
            IList<RutorListItem> list = await _rutorService.CheckListSettings(rutorCheckList);

            if (list != null)
                return Ok(list);

            return BadRequest("Не удалось заргузить страницу или XPath выражение ничего не нашло");
        }

        [HttpPost("CheckList")]
        public async Task<ActionResult<ParseResult>> CheckList([FromBody]RutorCheckList rutorCheckList)
        {
            ParseResult parseResult = await _rutorService.CheckList(rutorCheckList);

            return Ok(parseResult);
        }

        readonly IRutorService _rutorService;
    }
}