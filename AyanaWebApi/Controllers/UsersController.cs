using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AyanaWebApi.Models;

namespace AyanaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController()
        {

        }

        [HttpPost("auth")]
        public async Task<ActionResult<User>> Auth(User user)
        {
            return NotFound();
        }
    }
}