using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using PlayMakerApp.Models;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace PlayMakerApp.Controllers
{
    [Route("languages")]
    public class LanguageController : Controller
    {
        private readonly ILogger<LanguageController> _logger;

        public LanguageController(ILogger<LanguageController> logger)
        {
            _logger = logger;
        }

        [Route("change")]
        public IActionResult Change(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(6)});

            return RedirectToAction("Index", "Home");
        }
    }
}