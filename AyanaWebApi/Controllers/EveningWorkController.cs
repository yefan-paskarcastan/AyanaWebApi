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
    public class EveningWorkController : ControllerBase
    {
        public EveningWorkController(IEveningWorkService evService)
        {
            _evServices = evService;
        }

        [HttpPost("Publishing_{dayFromError}")]
        public async Task<ActionResult<string>> Publishing(int dayFromError)
        {
            bool res = await _evServices.Publishing(dayFromError);
            if (!res)
            {
                return "Ошибка при пакетной публикации постов";
            }
            return "Презентации успешно добавлены";
        }

        readonly IEveningWorkService _evServices;
    }
}
