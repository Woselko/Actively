using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using ActivelyApp.Models;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using ActivelyApp.Models.Common;
using Microsoft.Extensions.Options;
using Resources;

namespace ActivelyApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LanguageController : Controller
    {
        private readonly ILogger<LanguageController> _logger;
        private readonly List<string>? _supportedLanguages;

        public LanguageController(ILogger<LanguageController> logger, IOptions<LanguageSettings> languageSettings)
        {
            _logger = logger;
            _supportedLanguages = languageSettings.Value.SupportedLanguages;
        }

        [HttpGet]
        public IActionResult GetSupportedCultures()
        {
            if(_supportedLanguages == null)
            {
                return StatusCode(StatusCodes.Status404NotFound,
                        new Response { IsSuccess = false, Message = Common.SomethingWentWrong, });
            }
            return StatusCode(StatusCodes.Status200OK, _supportedLanguages);
        }

        [HttpPost]
        public IActionResult ChangeLanguageApi([FromBody]string culture)
        {
			if (culture == null || !_supportedLanguages.Contains(culture))
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new Response { IsSuccess = false, Message = Common.SomethingWentWrong,  });
            }
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(6) });

            return StatusCode(StatusCodes.Status200OK,
                    new Response { IsSuccess = true, Message = Common.Success,  });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ChangeLanguage(string culture)
        {
            if (culture == null || !_supportedLanguages.Contains(culture))
            {
                return BadRequest(StatusCodes.Status400BadRequest);
            }

            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(6)});

            return RedirectToAction("Index", "Home");
        }
    }
}