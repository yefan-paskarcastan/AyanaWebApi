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
    public class DriverController : ControllerBase
    {
        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        [HttpPost("RutorTorrent")]
        public async Task<ActionResult<TorrentSoftPost>> RutorTorrent(DriverRutorTorrentInput param)
        {
            TorrentSoftPost result = await _driverService.Convert(param);
            if (result != null)
            {
                foreach (var item in result.Screenshots)
                {
                    item.TorrentSoftPost = null;
                }
                return Ok(result);
            }
            return BadRequest("Сервису не удалось получить готовый пост");
        }

        readonly IDriverService _driverService;
    }
}
