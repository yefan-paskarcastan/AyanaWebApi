using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
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
    public class SoftController : ControllerBase
    {
        public SoftController(ISoftService softService)
        {
            _softService = softService;
        }

        [HttpPost("AddPost")]
        public async Task<ActionResult<string>> AddPost([FromBody]SoftPostInput inputParam)
        {
            PublishResult result = await _softService.AddPost(inputParam);
            if (result == PublishResult.Success)
                return Ok("Пост успешно добавлен");

            return BadRequest("Не удалось добавить пост");
        }

        readonly ISoftService _softService;
    }
}
