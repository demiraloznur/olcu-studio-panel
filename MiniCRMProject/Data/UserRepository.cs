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

        public bool Any()
        {
            using SqlConnection connection = db.Create();
            SqlCommand command = new SqlCommand("SELECT CASE WHEN EXISTS (SELECT 1 FROM Users) THEN 1 ELSE 0 END", connection);
            connection.Open();
            return (int)command.ExecuteScalar() == 1;
        }

        public bool EmailExists(string email)
        {
            using SqlConnection connection = db.Create();
            SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Email = @email", connection);
            command.Parameters.AddWithValue("@email", email.Trim());
            connection.Open();
            return (int)command.ExecuteScalar() > 0;
        }

        public UserModel? GetByEmail(string email)
        {
            using SqlConnection connection = db.Create();
            string sql = "SELECT UserId, FirstName, LastName, Email, PasswordHash, Role FROM Users WHERE Email = @email";
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
                PasswordHash = reader["PasswordHash"].ToString()!,
                Role = reader["Role"].ToString()!
            };
        }

        public int Add(UserModel user)
        {
            using SqlConnection connection = db.Create();
            string sql = @"INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Role)
                           OUTPUT INSERTED.UserId
                           VALUES (@firstName, @lastName, @email, @passwordHash, @role)";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@firstName", user.FirstName);
            command.Parameters.AddWithValue("@lastName", user.LastName);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@role", user.Role);

            connection.Open();
            try
            {
                return (int)command.ExecuteScalar();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // 2627/2601: benzersizlik kuralı ihlali (aynı e-posta)
                throw new DuplicateEmailException(ex);
            }
        }

        // Her müşteri ve proje sayıları; projesi olmayan müşteri 0 ile görünür.
        public List<CustomerSummary> GetCustomerSummaries()
        {
            List<CustomerSummary> list = new List<CustomerSummary>();
            using SqlConnection connection = db.Create();
            string sql = @"SELECT Users.UserId, Users.FirstName + ' ' + Users.LastName AS FullName, Users.Email,
                                  COUNT(Projects.ProjectId) AS ProjectCount,
                                  SUM(CASE WHEN Projects.ProjectId IS NOT NULL AND Projects.Stage <> 5 THEN 1 ELSE 0 END) AS ActiveProjectCount
                           FROM Users
                           LEFT JOIN Projects ON Projects.CustomerId = Users.UserId
                           WHERE Users.Role = 'Customer'
                           GROUP BY Users.UserId, Users.FirstName, Users.LastName, Users.Email
                           ORDER BY Users.FirstName, Users.LastName";
            SqlCommand command = new SqlCommand(sql, connection);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new CustomerSummary
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    FullName = reader["FullName"].ToString()!,
                    Email = reader["Email"].ToString()!,
                    ProjectCount = Convert.ToInt32(reader["ProjectCount"]),
                    ActiveProjectCount = Convert.ToInt32(reader["ActiveProjectCount"])
                });
            }
            return list;
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
