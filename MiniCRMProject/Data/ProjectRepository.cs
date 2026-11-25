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

        public List<ProjectModel> List(ProjectListQuery query)
        {
            List<ProjectModel> projects = new List<ProjectModel>();
            using SqlConnection connection = db.Create();
            var (whereAndOrder, parameters) = query.BuildWhereAndOrder();
            string sql = @"SELECT Projects.*, Users.FirstName + ' ' + Users.LastName AS CustomerFullName
                           FROM Projects
                           JOIN Users ON Projects.CustomerId = Users.UserId" + whereAndOrder;

            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddRange(parameters.ToArray());

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
            string sql = @"INSERT INTO Projects (ProjectName, Description, Stage, Category, District, CustomerId, CreatedAt, EndDate)
                           VALUES (@pName, @pDesc, @pStage, @pCategory, @pDistrict, @pCust, @pDate, @pEndDate)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@pName", project.ProjectName);
            command.Parameters.AddWithValue("@pDesc", project.Description ?? "");
            command.Parameters.AddWithValue("@pStage", (byte)project.Stage);
            command.Parameters.AddWithValue("@pCategory", (byte)project.Category);
            command.Parameters.AddWithValue("@pDistrict", string.IsNullOrWhiteSpace(project.District) ? DBNull.Value : project.District.Trim());
            command.Parameters.AddWithValue("@pCust", project.CustomerId);
            command.Parameters.AddWithValue("@pDate", project.CreatedAt == default ? DateTime.Now : project.CreatedAt);
            command.Parameters.AddWithValue("@pEndDate", project.EndDate.HasValue ? project.EndDate.Value : DBNull.Value);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Update(ProjectModel project)
        {
            using SqlConnection connection = db.Create();
            string sql = @"UPDATE Projects SET ProjectName = @name,
                           Description = @description,
                           Stage = @stage,
                           Category = @category,
                           District = @district,
                           EndDate = @endDate
                           WHERE ProjectId = @id";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@name", project.ProjectName);
            command.Parameters.AddWithValue("@description", project.Description ?? "");
            command.Parameters.AddWithValue("@stage", (byte)project.Stage);
            command.Parameters.AddWithValue("@category", (byte)project.Category);
            command.Parameters.AddWithValue("@district", string.IsNullOrWhiteSpace(project.District) ? DBNull.Value : project.District.Trim());
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
                Stage = (ProjectStage)Convert.ToByte(reader["Stage"]),
                Category = (ProjectCategory)Convert.ToByte(reader["Category"]),
                District = reader["District"] == DBNull.Value ? null : reader["District"].ToString(),
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
