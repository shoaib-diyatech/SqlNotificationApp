using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

class Program
{
    static void Main()
    {
        // #region SQL Server Connection String
        // string databaseServer = @"SHOAIB-KHALID\SQLEXPRESS";
        // string databaseName = "NotifyMe";
        // string userId = "sa"; // Or another SQL user
        // string password = "4Islamabad"; // Or another SQL user password
        // string connectionString = $"Data Source={databaseServer};Initial Catalog={databaseName};User ID={userId};Password={password};Encrypt=False;";
        // #endregion

        // Loading configuration from appsettings.json
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            DatabaseService databaseService = new DatabaseService(config);
            databaseService.Connect();
            //databaseService.RegisterDependency();
            databaseService.PopulateSubscriberTable(1000000);
    }
}