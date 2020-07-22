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
    public class ListPostsHandlerController : ControllerBase
    {
        public ListPostsHandlerController(IListPostsHandlerService listServices)
        {
            _listServices = listServices;
        }

        [HttpPost("Published")]
        public async Task<ActionResult<string>> Published([FromBody] TorrentSoftPostInput inputParam)
        {
            return await _listServices.Published(inputParam);
        }

        readonly IListPostsHandlerService _listServices;
    }
}
