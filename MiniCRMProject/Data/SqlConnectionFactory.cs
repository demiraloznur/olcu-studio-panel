using Microsoft.Data.SqlClient;

namespace MiniCRMProject.Data
{
    // Bağlantı cümlesini tek yerden okur; repository sınıfları bağlantıyı buradan alır.
    public class SqlConnectionFactory
    {
        private readonly string connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection bağlantı cümlesi bulunamadı.");
        }

        public SqlConnection Create()
        {
            return new SqlConnection(connectionString);
        }
    }
}
