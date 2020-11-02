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
    public class UIContentController : ControllerBase
    {
        readonly IUIContentService _contentService;

        public UIContentController(IUIContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpPost("Prepare")]
        public async Task<ActionResult<string>> Prepare()
        {
            bool res = _contentService.Prepare();
            return Ok("Выполнено успешно!");
        }
    }
}
