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
    public class ImghostController : ControllerBase
    {
        public ImghostController(IImghostService imghostService)
        {
            _imghost = imghostService;
        }

        [HttpPost("GetOriginalsUri")]
        public async Task<ActionResult<IList<string>>> GetOriginalsUri(ImghostGetOriginalsInput param)
        {
            IList<string> result = await _imghost.GetOriginalsUri(param);
            return Ok(result);
        }

        readonly IImghostService _imghost;
    }
}
