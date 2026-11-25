using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MiniCRMProject.Security
{
    // Yalnızca yöneticinin girebileceği sayfalar için. Oturum yoksa girişe,
    // müşteri ise ana sayfaya yönlendirir.
    public class RequireAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            SessionUser? user = SessionUser.From(context.HttpContext.Session);
            if (user == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
            else if (!user.IsAdmin)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
