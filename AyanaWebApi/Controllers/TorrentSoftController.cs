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
using Newtonsoft.Json;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TorrentSoftController : ControllerBase
    {
        public TorrentSoftController(ITorrentSoftService torrentSoftService)
        {
            _torrentSoftService = torrentSoftService;
        }

        [HttpPost("AddPost")]
        public async Task<ActionResult<string>> AddPost([FromBody]TorrentSoftAddPostInput inputParam)
        {
            TorrentSoftAddPostResult result = await _torrentSoftService.AddPost(inputParam);
            if (result == TorrentSoftAddPostResult.Success)
            {
                return Ok("Пост успешно добавлен");
            }
            return BadRequest("Не удалось добавить пост");
        }

        readonly ITorrentSoftService _torrentSoftService;
    }
}
