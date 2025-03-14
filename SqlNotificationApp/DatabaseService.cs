using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
public class DatabaseService
{
    private readonly string _connectionString;

    private bool _isConnected;

    SqlConnection _connection;

    public DatabaseService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
        _isConnected = false;
    }

    public void Connect()
    {
        try
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            _isConnected = true;
            Console.WriteLine("Connected to database!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection failed: {ex.Message}");
        }
    }

    public void TestConnection()
    {
        if (!_isConnected)
        {
            Console.WriteLine("Not connected to database. Trying to connect...");
            Connect();
            if (!_isConnected)
                return;
        }
        try
        {
            SqlCommand command = new SqlCommand("SELECT * from dbo.Subscriber;", _connection);
            int result = (int)command.ExecuteScalar();
            Console.WriteLine("Connection successful. Test query result: " + result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Test connection failed: {ex.Message}");
        }
    }

    public bool RegisterDependency()
    {

        if (!_isConnected)
        {
            Console.WriteLine("Not connected to database. Trying to connect...");
            Connect();
            if (!_isConnected){
                Console.WriteLine("Failed to connect to database. Cannot register dependency.");
                return false;
            }
        }

        string tableName = "dbo.Subscriber";
        try
        {
            // Start the SQL dependency listener
            Console.WriteLine("Starting SQL dependency listener...");
            SqlDependency.Start(_connectionString);
            Console.WriteLine("SQL dependency listener started.");

            // Ensure the database is enabled for notifications
            SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", _connection);
            SqlDependency dependency = new SqlDependency(command);
            dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);
            Console.WriteLine("SqlDependency created and event handler attached.");

            // Execute the command to register the notification
            command.ExecuteReader();
            Console.WriteLine("Command executed and notification registered.");

            // Keep the application running to listen for changes
            Console.WriteLine("Listening for changes. Press Enter to exit.");
            Console.ReadLine();

        }
        catch (SqlException sqlEx)
        {
            Console.WriteLine($"SQL error occurred: {sqlEx.Message}");
            return false;
        }
        catch (InvalidOperationException invalidOpEx)
        {
            Console.WriteLine($"Invalid operation: {invalidOpEx.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        finally
        {
            // Stop the SQL dependency listener
            Console.WriteLine("Stopping SQL dependency listener...");
            SqlDependency.Stop(_connectionString);
            Console.WriteLine("SQL dependency listener stopped.");
        }
        return true;
    }
    static void OnDependencyChange(object sender, SqlNotificationEventArgs e)
    {
        Console.WriteLine($"Data changed type: {e.Type}, info: {e.Info}, source: {e.Source}");
        // Handle the change notification (e.g., refresh data, send a message, etc.)
    }
}
