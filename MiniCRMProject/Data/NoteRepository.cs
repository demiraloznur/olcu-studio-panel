using Microsoft.Data.SqlClient;
using MiniCRMProject.Models;

namespace MiniCRMProject.Data
{
    public class NoteRepository
    {
        private readonly SqlConnectionFactory db;

        public NoteRepository(SqlConnectionFactory db)
        {
            this.db = db;
        }

        // Yeniden eskiye.
        public List<ProjectNote> ListForProject(int projectId)
        {
            List<ProjectNote> notes = new List<ProjectNote>();
            using SqlConnection connection = db.Create();
            string sql = "SELECT NoteId, ProjectId, Body, CreatedAt FROM ProjectNotes WHERE ProjectId = @projectId ORDER BY CreatedAt DESC, NoteId DESC";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@projectId", projectId);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                notes.Add(new ProjectNote
                {
                    NoteId = Convert.ToInt32(reader["NoteId"]),
                    ProjectId = Convert.ToInt32(reader["ProjectId"]),
                    Body = reader["Body"].ToString()!,
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }
            return notes;
        }

        public void Add(ProjectNote note)
        {
            using SqlConnection connection = db.Create();
            string sql = "INSERT INTO ProjectNotes (ProjectId, Body, CreatedAt) VALUES (@projectId, @body, @createdAt)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@projectId", note.ProjectId);
            command.Parameters.AddWithValue("@body", note.Body.Trim());
            command.Parameters.AddWithValue("@createdAt", note.CreatedAt == default ? DateTime.Now : note.CreatedAt);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
