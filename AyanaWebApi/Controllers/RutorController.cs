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

        [HttpPost("CheckList")]
        public async Task<ActionResult<ParseResult>> CheckList([FromBody]RutorCheckList rutorCheckList)
        {
            ParseResult parseResult = await _rutorService.CheckList(rutorCheckList);

            return Ok(parseResult);
        }

        readonly IRutorService _rutorService;
    }
}