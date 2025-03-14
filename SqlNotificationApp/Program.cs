using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

class Program
{
    static void Main()
    {
        #region SQL Server Connection String
        string databaseServer = @"SHOAIB-KHALID\SQLEXPRESS";
        string databaseName = "NotifyMe";
        string userId = "sa"; // Or another SQL user
        string password = "4Islamabad"; // Or another SQL user password
        string connectionString = $"Data Source={databaseServer};Initial Catalog={databaseName};User ID={userId};Password={password};Encrypt=False;";
        #endregion

        // Loading configuration from appsettings.json
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            DatabaseService databaseService = new DatabaseService(config);
            databaseService.Connect();
            databaseService.RegisterDependency();

        // string tableName = "dbo.Subscriber";
        // try
        // {
        //     // Test the connection and execute a simple query
        //     TestConnection(connectionString);

        //     // Start the SQL dependency listener
        //     Console.WriteLine("Starting SQL dependency listener...");
        //     SqlDependency.Start(connectionString);
        //     Console.WriteLine("SQL dependency listener started.");

        //     using (SqlConnection connection = new SqlConnection(connectionString))
        //     {
        //         connection.Open();
        //         Console.WriteLine("Connection opened.");

        //         // Ensure the database is enabled for notifications
        //         SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", connection);
        //         SqlDependency dependency = new SqlDependency(command);
        //         dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);
        //         Console.WriteLine("SqlDependency created and event handler attached.");

        //         // Execute the command to register the notification
        //         command.ExecuteReader();
        //         Console.WriteLine("Command executed and notification registered.");

        //         // Keep the application running to listen for changes
        //         Console.WriteLine("Listening for changes. Press Enter to exit.");
        //         Console.ReadLine();
        //     }
        // }
        // catch (SqlException sqlEx)
        // {
        //     Console.WriteLine($"SQL error occurred: {sqlEx.Message}");
        // }
        // catch (InvalidOperationException invalidOpEx)
        // {
        //     Console.WriteLine($"Invalid operation: {invalidOpEx.Message}");
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"An error occurred: {ex.Message}");
        // }
        // finally
        // {
        //     // Stop the SQL dependency listener
        //     Console.WriteLine("Stopping SQL dependency listener...");
        //     SqlDependency.Stop(connectionString);
        //     Console.WriteLine("SQL dependency listener stopped.");
        // }
    }

    // static void TestConnection(string connectionString)
    // {
    //     try
    //     {
    //         using (SqlConnection connection = new SqlConnection(connectionString))
    //         {
    //             connection.Open();
    //             SqlCommand command = new SqlCommand("SELECT 1", connection);
    //             int result = (int)command.ExecuteScalar();
    //             Console.WriteLine("Connection successful. Test query result: " + result);
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Test connection failed: {ex.Message}");
    //     }
    // }

    // static void OnDependencyChange(object sender, SqlNotificationEventArgs e)
    // {
    //     Console.WriteLine("Data changed!");
    //     // Handle the change notification (e.g., refresh data, send a message, etc.)
    // }
}