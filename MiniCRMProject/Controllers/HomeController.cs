using Microsoft.AspNetCore.Mvc;
using MiniCRMProject.Data;
using MiniCRMProject.Models;
using MiniCRMProject.Security;

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

        [RequireLogin]
        public IActionResult Index()
        {
            SessionUser user = SessionUser.From(HttpContext.Session)!;
            List<ProjectModel> list = projects.List(user.IsAdmin ? null : user.Id);

            ViewBag.UserFullName = user.FullName;
            ViewBag.UserRole = user.Role;

            return View(list);
        }

        [HttpGet]
        [RequireAdmin]
        public IActionResult Create()
        {
            ViewBag.Customers = users.GetCustomers();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAdmin]
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
        [RequireAdmin]
        public IActionResult Edit(int id)
        {
            ViewBag.UserRole = SessionUser.AdminRole;

            ProjectModel? model = projects.Get(id);
            if (model == null) return RedirectToAction("Index");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAdmin]
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
        [RequireAdmin]
        public IActionResult Delete(int id)
        {
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
