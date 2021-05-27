using ActionFilter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        [CustomizeAuthorize()]
        public IActionResult Index()
        {
            return Json("hihi");
        }
        [CustomizeAuthorize(1, 2)]
        public IActionResult Index2()
        {
            return Json("hihi2");
        }
    }
}
