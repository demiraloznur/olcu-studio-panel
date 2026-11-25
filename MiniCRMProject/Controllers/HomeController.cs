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
        private readonly NoteRepository notes;

        public HomeController(ProjectRepository projects, UserRepository users, NoteRepository notes)
        {
            this.projects = projects;
            this.users = users;
            this.notes = notes;
        }

        [RequireLogin]
        public IActionResult Index(ProjectListQuery query)
        {
            SessionUser user = SessionUser.From(HttpContext.Session)!;
            // Müşteri yalnızca kendi projelerini görür; bu değer formdan değil oturumdan gelir.
            query.CustomerId = user.IsAdmin ? null : user.Id;
            List<ProjectModel> list = projects.List(query);

            ViewBag.Query = query;

            ViewBag.UserFullName = user.FullName;
            ViewBag.UserRole = user.Role;

            return View(list);
        }

        [RequireLogin]
        public IActionResult Details(int id)
        {
            SessionUser user = SessionUser.From(HttpContext.Session)!;
            ProjectModel? project = projects.Get(id);
            if (project == null || !ProjectAccess.CanView(project, user)) return RedirectToAction("Index");

            ViewBag.Notes = notes.ListForProject(id);
            ViewBag.IsAdmin = user.IsAdmin;
            return View(project);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAdmin]
        public IActionResult AddNote(int projectId, string? body)
        {
            if (projects.Get(projectId) == null) return RedirectToAction("Index");

            ProjectNote note = new ProjectNote { ProjectId = projectId, Body = body?.Trim() ?? "" };
            if (!TryValidateModel(note))
            {
                TempData["NoteError"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
            }
            else
            {
                notes.Add(note);
            }
            return RedirectToAction("Details", new { id = projectId });
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
            if (!ModelState.IsValid)
            {
                ViewBag.Customers = users.GetCustomers();
                return View(projectModel);
            }

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
            if (!ModelState.IsValid)
            {
                ViewBag.UserRole = SessionUser.AdminRole;
                return View(projectModel);
            }

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
