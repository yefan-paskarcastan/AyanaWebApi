using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using AyanaWebApi.Services.Interfaces;
using AyanaWebApi.DTO;

namespace AyanaWebApi.Controllers
{
    //[Authorize]
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

        [HttpPost("GetPrograms")]
        public ActionResult<IList<ListItem>> GetPrograms(Pagination pagination)
        {
            if (pagination.CountItem < 1 || pagination.CurrentPage < 1)
            {
                pagination.CurrentPage = 1;
                pagination.CountItem = 50;
            }
            IList<ListItem> lst = _contentService.GetProgramsList(pagination);
            return Ok(lst);
        }
    }
}
