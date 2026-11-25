using Microsoft.AspNetCore.Mvc;
using MiniCRMProject.Data;
using MiniCRMProject.Security;

namespace MiniCRMProject.Controllers
{
    [RequireAdmin]
    public class CustomersController : Controller
    {
        private readonly UserRepository users;

        public CustomersController(UserRepository users)
        {
            this.users = users;
        }

        public IActionResult Index()
        {
            return View(users.GetCustomerSummaries());
        }
    }
}
