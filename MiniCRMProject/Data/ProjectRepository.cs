using Microsoft.Data.SqlClient;
using MiniCRMProject.Models;

namespace MiniCRMProject.Data
{
    public class ProjectRepository
    {
        private readonly SqlConnectionFactory db;

        public ProjectRepository(SqlConnectionFactory db)
        {
            this.db = db;
        }

        // customerId verilirse yalnızca o müşterinin projeleri döner.
        public List<ProjectModel> List(int? customerId)
        {
            List<ProjectModel> projects = new List<ProjectModel>();
            using SqlConnection connection = db.Create();
            string sql = @"SELECT Projects.*, Users.FirstName + ' ' + Users.LastName AS CustomerFullName
                           FROM Projects
                           JOIN Users ON Projects.CustomerId = Users.UserId";
            if (customerId.HasValue) sql += " WHERE Projects.CustomerId = @uid";

            SqlCommand command = new SqlCommand(sql, connection);
            if (customerId.HasValue) command.Parameters.AddWithValue("@uid", customerId.Value);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                ProjectModel model = Read(reader);
                model.CustomerFullName = reader["CustomerFullName"].ToString()!;
                projects.Add(model);
            }
            return projects;
        }

        public ProjectModel? Get(int id)
        {
            using SqlConnection connection = db.Create();
            string sql = "SELECT * FROM Projects WHERE ProjectId = @id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            return reader.Read() ? Read(reader) : null;
        }

        public void Add(ProjectModel project)
        {
            using SqlConnection connection = db.Create();
            string sql = @"INSERT INTO Projects (ProjectName, Description, Status, CustomerId, CreatedAt, EndDate)
                           VALUES (@pName, @pDesc, @pStatus, @pCust, @pDate, @pEndDate)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@pName", project.ProjectName);
            command.Parameters.AddWithValue("@pDesc", project.Description ?? "");
            command.Parameters.AddWithValue("@pStatus", project.Status);
            command.Parameters.AddWithValue("@pCust", project.CustomerId);
            command.Parameters.AddWithValue("@pDate", DateTime.Now);
            command.Parameters.AddWithValue("@pEndDate", project.EndDate.HasValue ? project.EndDate.Value : DBNull.Value);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Update(ProjectModel project)
        {
            using SqlConnection connection = db.Create();
            string sql = @"UPDATE Projects SET ProjectName = @name,
                           Description = @description,
                           Status = @status,
                           EndDate = @endDate
                           WHERE ProjectId = @id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", project.ProjectName);
            command.Parameters.AddWithValue("@description", project.Description ?? "");
            command.Parameters.AddWithValue("@status", project.Status);
            command.Parameters.AddWithValue("@id", project.ProjectId);
            command.Parameters.AddWithValue("@endDate", project.EndDate.HasValue ? project.EndDate.Value : DBNull.Value);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using SqlConnection connection = db.Create();
            string sql = "DELETE FROM Projects WHERE ProjectId = @id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }

        private static ProjectModel Read(SqlDataReader reader)
        {
            ProjectModel model = new ProjectModel
            {
                ProjectId = Convert.ToInt32(reader["ProjectId"]),
                ProjectName = reader["ProjectName"].ToString()!,
                Description = reader["Description"].ToString()!,
                Status = reader["Status"].ToString()!,
                CustomerId = Convert.ToInt32(reader["CustomerId"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };
            if (reader["EndDate"] != DBNull.Value)
            {
                model.EndDate = Convert.ToDateTime(reader["EndDate"]);
            }
            return model;
        }
    }
}
