using Microsoft.AspNetCore.Mvc;
using MiniCRMProject.Data;
using MiniCRMProject.Models;
using MiniCRMProject.Security;

namespace MiniCRMProject.Controllers
{
    [RequireAdmin]
    public class DashboardController : Controller
    {
        private readonly ProjectRepository projects;

        public DashboardController(ProjectRepository projects)
        {
            this.projects = projects;
        }

        public IActionResult Index()
        {
            List<ProjectModel> all = projects.List(new ProjectListQuery());
            return View(DashboardSummary.From(all, DateTime.Today));
        }
    }
}
