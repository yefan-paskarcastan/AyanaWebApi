using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AyanaWebApi.Models;
using Microsoft.Extensions.Options;

namespace AyanaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController(AyDbContext dbContext,
                               IOptions<JWTSettings> jwtSettings)
        {
            _context = dbContext;
            _jwtSettings = jwtSettings.Value;
        }

        #region Private
        readonly AyDbContext _context;

        readonly JWTSettings _jwtSettings;
        #endregion
    }
}