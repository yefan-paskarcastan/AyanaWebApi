using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class NnmclubController : ControllerBase
    {
        public NnmclubController(INnmclubService nnmclubService)
        {
            _nnmclubService = nnmclubService;
        }

        [HttpPost("ParseItem")]
        public async Task<ActionResult<NnmclubItem>> ParseItem([FromBody] NnmclubParseItemInput param)
        {
            NnmclubItem item = await _nnmclubService.ParseItem(param);
            if (item != null)
            {
                foreach (var img in item.Imgs)
                {
                    img.NnmclubItem = null;
                }
                foreach (var spl in item.Spoilers)
                {
                    spl.NnmclubItem = null;
                }
                item.NnmclubListItem = null;
                return Ok(item);
            }
            return BadRequest("Не удалось распарсить");
        }

        [HttpPost("CheckList")]
        public async Task<ActionResult<IList<NnmclubListItem>>> CheckList([FromBody]NnmclubCheckListInput input)
        {
            IList<NnmclubListItem> result = await _nnmclubService.CheckList(input);

            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("При проверке списка раздач произошла ошибка");
        }

        readonly INnmclubService _nnmclubService;
    }
}
