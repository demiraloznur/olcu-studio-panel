using Microsoft.AspNetCore.Mvc;
using MiniCRMProject.Data;
using MiniCRMProject.Models;

namespace MiniCRMProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProjectRepository projects;
        private readonly UserRepository users;

        public HomeController(ProjectRepository projects, UserRepository users)
        {
            this.projects = projects;
            this.users = users;
        }

        public IActionResult Index()
        {
            string? role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Account");

            int? userId = HttpContext.Session.GetInt32("UserId");
            List<ProjectModel> list = projects.List(role == "Customer" ? userId : null);

            ViewBag.UserFullName = HttpContext.Session.GetString("UserFullName");
            ViewBag.UserRole = role;

            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Index");

            ViewBag.Customers = users.GetCustomers();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProjectModel projectModel)
        {
            try
            {
                projects.Add(projectModel);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Yeni proje kaydedilirken veritabanı kaynaklı bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            ProjectModel model = projects.Get(id) ?? new ProjectModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProjectModel projectModel)
        {
            try
            {
                projects.Update(projectModel);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Proje güncellenirken veritabanı kaynaklı bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Index");

            try
            {
                projects.Delete(id);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Proje silinirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }
    }
}
