using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AyanaWebApi.Models;
using AyanaWebApi.Services.Interfaces;

namespace AyanaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TegreiUIController : ControllerBase
    {
        public TegreiUIController(ITegreiUIService tegreiUIService)
        {
            _uiService = tegreiUIService;
        }

        [HttpPost("rutorList")]
        public async Task<ActionResult<IList<RutorItem>>> RutorList()
        {
            IList<RutorItem> lst = await _uiService.RutorList();
            return Ok(lst);
        }

        ITegreiUIService _uiService;
    }
}
