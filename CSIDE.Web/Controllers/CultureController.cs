using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CSIDE.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        public IActionResult Set(string culture, string redirectUri)
        {
            if (culture != null)
            {
                var requestCulture = new RequestCulture(culture, culture);
                var cookieName = CookieRequestCultureProvider.DefaultCookieName;
                var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);
                var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions() { Secure = true };

                HttpContext.Response.Cookies.Append(cookieName, cookieValue, cookieOptions);
            }

            return LocalRedirect(redirectUri);
        }
    }
}
