using Microsoft.AspNetCore.Mvc;
using ActivelyApp.Models;
using System.Diagnostics;
using ActivelyApp.Models.Common;

namespace ActivelyApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("~/")]
        [Route("/Home")]
        [Route("~/Home/Index")]
        public IActionResult Index()
        {
            return View();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Privacy()
        {
            return View();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}