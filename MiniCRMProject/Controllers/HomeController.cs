using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MiniCRMProject.Models;

namespace MiniCRMProject.Controllers
{
    public class HomeController : Controller
    {
        string connectionString;

        public HomeController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            string role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Account");

            List<ProjectModel> projects = new List<ProjectModel>();
            int? userId = HttpContext.Session.GetInt32("UserId");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = @"SELECT Projects.*, Users.FirstName + ' ' + Users.LastName as CustomerFullName 
                               FROM Projects 
                               JOIN Users ON Projects.CustomerId = Users.UserId";

                if (role == "Customer")
                {
                    sql += " WHERE Projects.CustomerId = @uid";
                }

                SqlCommand command = new SqlCommand(sql, connection);

                if (role == "Customer")
                {
                    command.Parameters.AddWithValue("@uid", userId);
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ProjectModel model = new ProjectModel();
                    model.ProjectId = Convert.ToInt32(reader["ProjectId"]);
                    model.ProjectName = reader["ProjectName"].ToString();
                    model.Description = reader["Description"].ToString();
                    model.Status = reader["Status"].ToString();
                    model.CustomerId = Convert.ToInt32(reader["CustomerId"]);
                    model.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);

                    if (reader["EndDate"] != DBNull.Value)
                    {
                        model.EndDate = Convert.ToDateTime(reader["EndDate"]);
                    }

                    model.CustomerFullName = reader["CustomerFullName"].ToString();
                    projects.Add(model);
                }
            }

            ViewBag.UserFullName = HttpContext.Session.GetString("UserFullName");
            ViewBag.UserRole = role;

            return View(projects);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Index");

            List<UserModel> customers = new List<UserModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT UserId, FirstName, LastName FROM Users WHERE Role = 'Customer'";
                SqlCommand command = new SqlCommand(sql, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    UserModel customer = new UserModel();
                    customer.UserId = (int)reader["UserId"];
                    customer.FirstName = reader["FirstName"].ToString();
                    customer.LastName = reader["LastName"].ToString();
                    customers.Add(customer);
                }
            }
            ViewBag.Customers = customers;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProjectModel projectModel)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = @"INSERT INTO Projects (ProjectName, Description, Status, CustomerId, CreatedAt, EndDate) 
                           VALUES (@pName, @pDesc, @pStatus, @pCust, @pDate, @pEndDate)";

                    SqlCommand command = new SqlCommand(sql, connection);

                    command.Parameters.AddWithValue("@pName", projectModel.ProjectName);

                    string desc = projectModel.Description;
                    if (desc == null) desc = "";

                    command.Parameters.AddWithValue("@pDesc", desc);
                    command.Parameters.AddWithValue("@pStatus", projectModel.Status);
                    command.Parameters.AddWithValue("@pCust", projectModel.CustomerId);
                    command.Parameters.AddWithValue("@pDate", DateTime.Now);

                    if (projectModel.EndDate.HasValue)
                        command.Parameters.AddWithValue("@pEndDate", projectModel.EndDate.Value);
                    else
                        command.Parameters.AddWithValue("@pEndDate", DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
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

            ProjectModel model = new ProjectModel();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Projects WHERE ProjectId = @id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    model.ProjectId = Convert.ToInt32(reader["ProjectId"]);
                    model.ProjectName = reader["ProjectName"].ToString();
                    model.Description = reader["Description"].ToString();
                    model.Status = reader["Status"].ToString();
                    model.CustomerId = Convert.ToInt32(reader["CustomerId"]);
                    model.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);

                    if (reader["EndDate"] != DBNull.Value)
                    {
                        model.EndDate = Convert.ToDateTime(reader["EndDate"]);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProjectModel projectModel)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = @"UPDATE Projects SET ProjectName = @name, 
                           Description = @description, 
                           Status = @status,
                           EndDate = @endDate
                           WHERE ProjectId = @id";

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@name", projectModel.ProjectName);

                    string desc = projectModel.Description;
                    if (desc == null) desc = "";

                    command.Parameters.AddWithValue("@description", desc);
                    command.Parameters.AddWithValue("@status", projectModel.Status);
                    command.Parameters.AddWithValue("@id", projectModel.ProjectId);

                    if (projectModel.EndDate.HasValue)
                        command.Parameters.AddWithValue("@endDate", projectModel.EndDate.Value);
                    else
                        command.Parameters.AddWithValue("@endDate", DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = "DELETE FROM Projects WHERE ProjectId = @id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Proje silinirken bir hata oluştu.";
            }

            return RedirectToAction("Index");
        }
    }
}

