using ActionFilter;
using Business.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
       
        private readonly ILogger<LoginController> _logger;
        private readonly IUser _user;

        public LoginController(ILogger<LoginController> logger, IUser user)
        {
            _logger = logger;
            _user = user;
        }
        [HttpPost]
        [Route("login")]
        public ActionResult Login(string userName, string password)
        {
            var loginResult = _user.Login(userName, password, false);
            return Ok(loginResult);
        }
        [HttpPost]
        [Route("logout")]
        [CustomizeAuthorize]
        public ActionResult Logout(string userName, string password)
        {
            var logoutResult = _user.Logout();
            return Ok(logoutResult);
        }
        [HttpPost]
        [Route("refreshtoken")]
        public ActionResult RefreshToken(string refreshToken)
        {
            var result = _user.RefresToken(refreshToken);
            return Ok(result);
        }
    }
}
