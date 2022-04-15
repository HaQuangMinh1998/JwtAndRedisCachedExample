using ActionFilter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ResourceController : Controller
    {
        private readonly ILogger<ResourceController> _logger;

        public ResourceController(ILogger<ResourceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("index")]
        [CustomizeAuthorize()]
        public IActionResult Index()
        {
            return Json("hihi");
        }
        [Route("index2")]
        [CustomizeAuthorize(4,5)]
        public IActionResult Index2()
        {
            return Json("hihi2");
        }
        [Route("index3")]
       // [CustomizeAuthorize(1, 3)]
        public IActionResult Index3()
        {
            return Json("hihi3");
        }
    }
}
