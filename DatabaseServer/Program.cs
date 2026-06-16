using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Welcome to the database server");
bool running = true;
while (running)
{
    Console.WriteLine("Main menu:");
    Console.WriteLine("1) Create database");
    Console.WriteLine("2) Check the balance");
    Console.WriteLine("3) Add transaction");
    Console.WriteLine("4) Pay day");
    Console.WriteLine("5) Delete database");
    Console.WriteLine("Q: Quit");

    string input = Console.ReadLine().ToUpper();
    if(input == "Q")
    {
        running = false;
    }

    if(input == "5")
    {
        Console.Write("Are you sure? Type YES to confirm. This cannot be undone: ");
        if(Console.ReadLine() == "YES")
        {
            File.Delete("finances.db");
        }
        
    }

    // Create the database
    if (input == "1")
    {
        SqliteConnection c = new SqliteConnection("Data Source=finances.db");
        c.Open();
        string SQL = File.ReadAllText("initialdata.txt");
        Console.WriteLine($"Running SQL: {SQL}");
        
        SqliteCommand cmd = c.CreateCommand();
        cmd.CommandText = SQL;
        try
        {
            cmd.ExecuteNonQuery();
        } catch(SqliteException e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        c.Close();
    }

    // Check the balanace
    if (input == "2")
    {
        SqliteConnection c = new SqliteConnection("Data Source=finances.db");
        c.Open();
        SqliteCommand cmd = c.CreateCommand();
        Console.Write("username: ");
        string username = Console.ReadLine();
        cmd.CommandText = $"SELECT * FROM Person WHERE username = '{username}'";
        SqliteDataReader r = cmd.ExecuteReader();
        if(r.HasRows)
        {
            r.Read();
            int id = r.GetInt32(0);
            string firstname = r.GetString(1);
            string lastname = r.GetString(2);
            string password = r.GetString(4);
            int jobID = r.GetInt32(5);

            int balance = 0;

            Console.WriteLine($"{id,3}|{firstname,15}|{lastname,15}|{username,15}|{password,15}");

            // get list of money received
            string sql = $"SELECT Payment.*, Person.FirstName, Person.LastName FROM Payment " +
                "JOIN Person ON Payment.RecipientID = Person.PersonID " +
                $"WHERE GiverID={id}";
            SqliteCommand transactions = c.CreateCommand();
            transactions.CommandText = sql;
            SqliteDataReader rTransactions = transactions.ExecuteReader();
            Console.WriteLine($"\nPayments received:");
            while(rTransactions.Read())
            {
                int PaymentID = rTransactions.GetInt32(0);
                int GiverID = rTransactions.GetInt32(1);
                int RecipientID = rTransactions.GetInt32(2);
                int Amount = rTransactions.GetInt32(3);
                string Date = rTransactions.GetString(4);
                string Description = rTransactions.GetString(5);
                string FirstName = rTransactions.GetString(6);
                string LastName = rTransactions.GetString(7);
                balance -= Amount;

                Console.WriteLine($"{PaymentID,3}| Sent £{Amount} to ({RecipientID}) {FirstName} {LastName} for {Description}, date: {Date}");


            }
            // List of money sent
            
            sql = $"SELECT Payment.*, Person.FirstName, Person.LastName FROM Payment " +
                "JOIN Person ON Payment.GiverID = Person.PersonID " +
                $"WHERE RecipientID={id}";
            transactions = c.CreateCommand();
            transactions.CommandText = sql;
            rTransactions = transactions.ExecuteReader();
            Console.WriteLine($"\nPayments sent:");
            while (rTransactions.Read())
            {
                int PaymentID = rTransactions.GetInt32(0);
                int GiverID = rTransactions.GetInt32(1);
                int RecipientID = rTransactions.GetInt32(2);
                int Amount = rTransactions.GetInt32(3);
                string Date = rTransactions.GetString(4);
                string Description = rTransactions.GetString(5);
                string FirstName = rTransactions.GetString(6);
                string LastName = rTransactions.GetString(7);
                balance += Amount;
                Console.WriteLine($"{PaymentID,3}| Recieved £{Amount} from ({GiverID}) {FirstName} {LastName} for {Description}, date: {Date}");

            }

            Console.WriteLine($"\nTotal balance: £{balance}\n");

        } else
        {
            Console.WriteLine($"No user found: {username}");
        }
            c.Close();
    }
}
