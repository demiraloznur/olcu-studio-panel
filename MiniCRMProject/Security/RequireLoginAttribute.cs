using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MiniCRMProject.Security
{
    // Oturum açmamış kullanıcıyı giriş sayfasına yönlendirir.
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (SessionUser.From(context.HttpContext.Session) == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
