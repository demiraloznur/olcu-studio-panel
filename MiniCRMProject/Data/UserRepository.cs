using Microsoft.Data.SqlClient;
using MiniCRMProject.Models;

namespace MiniCRMProject.Data
{
    public class UserRepository
    {
        private readonly SqlConnectionFactory db;

        public UserRepository(SqlConnectionFactory db)
        {
            this.db = db;
        }

        public UserModel? GetByEmail(string email)
        {
            using SqlConnection connection = db.Create();
            string sql = "SELECT UserId, FirstName, LastName, Email, Password, Role FROM Users WHERE Email = @email";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@email", email);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read()) return null;

            return new UserModel
            {
                UserId = Convert.ToInt32(reader["UserId"]),
                FirstName = reader["FirstName"].ToString()!,
                LastName = reader["LastName"].ToString()!,
                Email = reader["Email"].ToString()!,
                Password = reader["Password"].ToString()!,
                Role = reader["Role"].ToString()!
            };
        }

        public int Add(UserModel user)
        {
            using SqlConnection connection = db.Create();
            string sql = @"INSERT INTO Users (FirstName, LastName, Email, Password, Role)
                           OUTPUT INSERTED.UserId
                           VALUES (@firstName, @lastName, @email, @password, @role)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@firstName", user.FirstName);
            command.Parameters.AddWithValue("@lastName", user.LastName);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@password", user.Password);
            command.Parameters.AddWithValue("@role", user.Role);

            connection.Open();
            return (int)command.ExecuteScalar();
        }

        public List<UserModel> GetCustomers()
        {
            List<UserModel> customers = new List<UserModel>();
            using SqlConnection connection = db.Create();
            string sql = "SELECT UserId, FirstName, LastName FROM Users WHERE Role = 'Customer' ORDER BY FirstName, LastName";
            SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                customers.Add(new UserModel
                {
                    UserId = (int)reader["UserId"],
                    FirstName = reader["FirstName"].ToString()!,
                    LastName = reader["LastName"].ToString()!
                });
            }
            return customers;
        }
    }
}
