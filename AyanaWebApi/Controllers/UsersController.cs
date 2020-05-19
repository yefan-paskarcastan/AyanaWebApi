using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AyanaWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using AyanaWebApi.Utils;

namespace AyanaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<ActionResult<User>> Auth([FromBody]User user)
        {
            user = await _userService.Authenticate(user.Login, user.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [HttpPost("regist")]
        public async Task<ActionResult<User>> Regist([FromBody]User user)
        {
            try
            {
                User createdUser = await _userService.Create(user);
                return Ok(createdUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        readonly IUserService _userService;
    }
}