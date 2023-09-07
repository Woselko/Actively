using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using ActivelyApp.Models;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using Microsoft.Extensions.Options;
using Resources;
using ActivelyApp.Models.Common;

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

        [HttpPost]
        [Route("changelanguageapi")]
        public IActionResult ChangeLanguageApi(string culture)
        {
            if (culture == null || !_supportedLanguages.Contains(culture))
            {
                return StatusCode(StatusCodes.Status404NotFound,
                    new Response { Status = Common.Error, Message = Common.SomethingWentWrong, Type = ResponseType.Error });
            }
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(6) });

            return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = Common.Success, Message = Common.Success, Type = ResponseType.Succes });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("changelanguage")]
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