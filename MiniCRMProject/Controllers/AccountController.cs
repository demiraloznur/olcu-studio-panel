using Microsoft.AspNetCore.Mvc;
using MiniCRMProject.Data;
using MiniCRMProject.Models;
using MiniCRMProject.Security;
using MiniCRMProject.Services;

namespace MiniCRMProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository users;
        private readonly ProjectRepository projects;
        private readonly PasswordService passwords;

        public AccountController(UserRepository users, ProjectRepository projects, PasswordService passwords)
        {
            this.users = users;
            this.projects = projects;
            this.passwords = passwords;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid && users.EmailExists(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Bu e-posta ile zaten bir hesap var.");
            }
            if (!ModelState.IsValid) return View(model);

            try
            {
                users.Add(new UserModel
                {
                    FirstName = model.FirstName.Trim(),
                    LastName = model.LastName.Trim(),
                    Email = model.Email.Trim(),
                    PasswordHash = passwords.Hash(model.Password),
                    Role = SessionUser.CustomerRole
                });
            }
            catch (DuplicateEmailException ex)
            {
                // Aynı anda gelen başka bir kayıt aynı e-postayı az önce almış olabilir.
                ModelState.AddModelError(nameof(model.Email), ex.Message);
                return View(model);
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string? email, string? password)
        {
            UserModel? user = string.IsNullOrWhiteSpace(email) ? null : users.GetByEmail(email.Trim());

            if (user != null && passwords.Verify(user.PasswordHash, password ?? ""))
            {
                SessionUser sessionUser = new SessionUser(user.UserId, user.FirstName + " " + user.LastName, user.Role);
                sessionUser.WriteTo(HttpContext.Session);

                if (!sessionUser.IsAdmin)
                {
                    List<ProjectModel> own = projects.List(new ProjectListQuery { CustomerId = sessionUser.Id });
                    if (own.Count == 1) return RedirectToAction("Details", "Home", new { id = own[0].ProjectId });
                }
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "E-posta veya şifre hatalı. Lütfen tekrar deneyin.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
