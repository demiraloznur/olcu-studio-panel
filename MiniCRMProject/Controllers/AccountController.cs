using Microsoft.AspNetCore.Mvc;
using MiniCRMProject.Data;
using MiniCRMProject.Models;
using MiniCRMProject.Services;

namespace MiniCRMProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository users;
        private readonly PasswordService passwords;

        public AccountController(UserRepository users, PasswordService passwords)
        {
            this.users = users;
            this.passwords = passwords;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserModel user, string password)
        {
            user.PasswordHash = passwords.Hash(password);
            user.Role = "Customer";
            users.Add(user);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            UserModel? user = users.GetByEmail(email);

            if (user != null && passwords.Verify(user.PasswordHash, password))
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("UserFullName", user.FirstName + " " + user.LastName);
                HttpContext.Session.SetString("UserRole", user.Role);

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
