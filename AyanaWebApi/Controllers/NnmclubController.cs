using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;
using AyanaWebApi.Utils;

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

        [HttpPost("CheckList")]
        public async Task<ActionResult<IList<NnmclubListItem>>> CheckList([FromBody]NnmclubCheckListInput input)
        {
            ServiceResult<IList<NnmclubListItem>> result = await _nnmclubService.CheckList(input);

            if (result.ResultObj != null)
            {
                return Ok(result.ResultObj);
            }
            return BadRequest("При проверке списка раздач произошла ошибка");
        }

        readonly INnmclubService _nnmclubService;
    }
}
