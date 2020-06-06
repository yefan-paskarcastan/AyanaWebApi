using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AyanaWebApi.Services;
using AyanaWebApi.Models;

namespace AyanaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DriverController : ControllerBase
    {
        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpPost("RutorTorrent")]
        public async Task<ActionResult<string>> RutorTorrent(DriverRutorTorrentInput param)
        {
            string result = await _driverService.RutorTorrent(param);
            return BadRequest($"Не удалось добавить пост {result}");
        }

        readonly IDriverService _driverService;
    }
}
