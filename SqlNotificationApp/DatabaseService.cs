using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public void Connect()
    {
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            Console.WriteLine("Connected to database!");
        }
    }
}
