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
        public ActionResult DoLogin(string userName, string password)
        {
            var loginResult = _user.Login(userName, password, false);
            return Ok(loginResult);
        }
    }
}
