using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ImgsConverterController : ControllerBase
    {
        public ImgsConverterController(IImgsConverterService converterService)
        {
            _converterService = converterService;
        }

        [HttpGet("ToJPG_{quality}")]
        public ActionResult<string> ToJPG([FromBody]string fullFileName, int quality)
        {
            string result = _converterService.ConvertToJpg(fullFileName, quality);
            return Ok(result);
        }

        IImgsConverterService _converterService;
    }
}
