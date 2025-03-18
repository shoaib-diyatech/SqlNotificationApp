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
            if (!_isConnected)
            {
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
            SqlCommand command = new SqlCommand($"select * from dbo.Subscriber where id = 1;", _connection);
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

    public void PopulateSubscriberTable(long count)
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
            SqlCommand sqlCommand;
            //SqlCommand command = new SqlCommand("TRUNCATE TABLE dbo.Subscriber;", _connection);
            //command.ExecuteNonQuery();
            // Console.WriteLine("Table cleared.");

            for (long i = 1; i <= count; i++)
            {
                try
                {
                    Subscriber sub = Subscriber.GetRandomSubscriber();
                    //sqlCommand = new SqlCommand($"INSERT INTO dbo.Subscriber (msisdn, name, isActive, email, dateOfBirth, id) VALUES ('{sub.Msisdn}', '{sub.Name}', {sub.IsActive}, '{sub.Email}', '{sub.DateOfBirth}', {sub.Id});", _connection);

                    sqlCommand = new SqlCommand("INSERT INTO dbo.Subscriber (msisdn, name, isActive, email, dateOfBirth, id) VALUES (@Msisdn, @Name, @IsActive, @Email, @DateOfBirth, @Id);", _connection);
                    sqlCommand.Parameters.AddWithValue("@Msisdn", sub.Msisdn);
                    sqlCommand.Parameters.AddWithValue("@Name", sub.Name);
                    sqlCommand.Parameters.AddWithValue("@IsActive", sub.IsActive);
                    sqlCommand.Parameters.AddWithValue("@Email", sub.Email);
                    sqlCommand.Parameters.AddWithValue("@DateOfBirth", sub.DateOfBirth);
                    sqlCommand.Parameters.AddWithValue("@Id", sub.Id);
                    sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Insert failed: {ex.Message}");
                }
            }
            Console.WriteLine($"Table populated with {count} records.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Populate table failed: {ex.Message}");
        }

    }
}

public class Subscriber
{
    public string Msisdn { get; set; }
    public string Name { get; set; }

    //public Gender Gender { get; set; }
    public string Email { get; set; }

    public bool IsActive { get; set; }

    public DateOnly DateOfBirth { get; set; }
    //public ServiceType ServiceType { get; set; }
    public long Id { get; set; }

    /// <summary>
    /// Serialize the Subscriber object to JSON
    /// </summary>
    /// <param name="subscriber"></param>
    /// <returns></returns>
    // public static string Serialize(Subscriber subscriber)
    // {
    //     return JsonConvert.SerializeObject(subscriber);
    // }

    // /// <summary>
    // /// Deserialize JSON to a Subscriber object
    // /// </summary>
    // /// <param name="json"></param>
    // /// <returns></returns>
    // public static Subscriber Deserialize(string json)
    // {
    //     return JsonConvert.DeserializeObject<Subscriber>(json);
    // }

    public override string ToString()
    {
        return $"Subscriber: {Name} ({Msisdn})";
    }

    public static Subscriber GetRandomSubscriber()
    {
        Random random = new Random();
        int randomDay = random.Next(1, 28);
        int randomMonth = random.Next(1, 12);
        int randomYear = random.Next(1970, 2000);
        int id = random.Next(1000, 9999);
        string[] randomNames = { "John Doe", "Jane Doe", "Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Heidi",
            "Ivan", "Jack", "Kathy", "Liam", "Mia", "Nina", "Oliver", "Pam", "Quinn", "Riley", "Sara", "Tom", "Uma", "Vera", "Will", "Xander", "Yara", "Zara", "Zoe" , "Zack"
            , "Fever", "Frost", "Gale", "Garnet", "Ginger", "Gizmo", "Goblin", "Goldie", "Goober", "Goose", "Gopher", "Grizzly", "Gulliver", "Guppy", "Gus", "Gypsy", "Haley", "Hank", "Harley", "Harper", "Hawk", "Hazel", "Heath", "Hercules", "Hershey", "Holly", "Honey", "Honor", "Hope", "Hudson", "Hunter", "Iggy", "Indigo", "Inky", "Iris", "Isis", "Ivory", "Ivy", "Jade", "Jagger", "Jaguar", "Jazz", "Jellybean", "Jewel", "Jinx", "Joey", "Journey", "Joy", "Jude", "Jules", "Juliet", "June", "Juno", "Jupiter", "Kai", "Karma", "Kash", "Keanu", "Keats", "Keiko", "Kiki", "King", "Kip", "Kismet", "Klaus", "Koda", "Kodiak", "Kona", "Kosmo", "Kovu", "Kuma", "Kyoto", "Lark", "Layla", "Legend", "Lemon", "Leo", "Levi", "Lilac", "Lily", "Lincoln", "Lionel", "Lola", "Lucky", "Lulu", "Luna", "Lyric", "Mabel", "Mack", "Maddox", "Maggie", "Mango", "Maple", "Marley", "Mars", "Maverick", "Max", "Maya", "Meadow", "Mercury", "Mia", "Midnight", "Mika", "Miles", "Milo", "Mimi", "Minnie", "Misty", "Mocha", "Moe", "Molly", "Monkey", "Monty", "Moon", "Moose", "Morgan", "Mowgli", "Muffin", "Mulan", "Munchkin", "Murray", "Mya",
            "Nala", "Nash", "Nell", "Nemo", "Neptune", "Nico", "Nikita", "Niko", "Nina", "Ninja", "Noah", "Nola", "Noodle", "Nugget", "Nutmeg", "Oakley", "Oasis", "Odie", "Olive", "Olivia", "Ollie", "Onyx", "Opal", "Oreo", "Orion", "Otis", "Otto", "Ozzy", "Pablo", "Paisley", "Panda", "Pandora", "Pansy", "Parker", "Pasha", "Peanut", "Pearl", "Pebbles", "Penny", "Pepper", "Petunia", "Phoebe", "Pickle", "Piper", "Pippin", "Pistol", "Pixie", "Pluto", "Pogo", "Polly", "Poppy", "Porter", "Posey", "Preston", "Prince", "Princess", "Priscilla", "Puck", "Puddles", "Pumpkin", "Puppy", "Pyro", "Quincy", "Quinn", "Radar", "Ralph", "Ranger", "Rascal", "Raven", "Rebel", "Reese", "Rex", "Rhubarb", "Rico", "Riley", "Ringo", "Rio", "Ripley", "River", "Rocco", "Rocky", "Rogue", "Romeo", "Roo", "Roscoe", "Rose", "Rosie", "Rover", "Ruby", "Rudy", "Rufus", "Rusty", "Sadie", "Sage", "Sahara", "Salem", "Sam", "Samantha", "Sammy", "Samson", "Sandy", "Sapphire", "Sarge", "Sasha", "Sassy", "Scooby", "Scout", "Scrappy", "Shadow", "Shasta", "Shelby", "Sherlock", "Shiloh", "Simba", "Sissy", "Skippy", "Sky", "Smokey", "Snickers",
            "Snoopy", "Snowball", "Socks", "Sophie", "Sparrow", "Spencer", "Spike", "Spirit", "Spot", "Sprout", "Squirt", "Stella", "Stitch", "Storm", "Sugar", "Suki", "Sully", "Sunny", "Sunshine", "Susie", "Sylvester", "Taco", "Taffy", "Tasha", "Taz", "Teddy", "Tesla", "Theo", "Thor", "Tiger", "Tilly", "Timber", "Toby", "Tucker", "Tulip",
            "Turtle", "Kuala", "Kangaroo", "Koala", "Kookaburra", "Koel", "Justin", "George", "Peter", "Jane"};
        string name = randomNames[random.Next(0, randomNames.Length)];
        return new Subscriber
        {
            Msisdn = "+1" + random.Next(100000000, 999999999).ToString(),
            Id = id,
            DateOfBirth = new DateOnly(randomYear, randomMonth, randomDay),
            Name = name,
            IsActive = id % 2 == 0,
            Email = name + id + "@example.com",
        };
    }

    public int GetAge()
    {

        return DateTime.Now.Year - DateOfBirth.Year;

    }
}
