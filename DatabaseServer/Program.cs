using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;

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

            Console.WriteLine($"{id,5}|{firstname,15}|{lastname,15}|{username,15}:{password,15}");

            // get list of money received
            string sql = $"SELECT * FROM Payment WHERE RecipientID={id}";
            SqliteCommand transactions = c.CreateCommand();
            transactions.CommandText = sql;
            SqliteDataReader rTransactions = transactions.ExecuteReader();
            Console.WriteLine($"Payments received:");
            while(rTransactions.Read())
            {
                int PaymentID = rTransactions.GetInt32(0);
                int GiverID = rTransactions.GetInt32(1);
                int RecipientID = rTransactions.GetInt32(2);
                int Amount = rTransactions.GetInt32(3);
                string Description = rTransactions.GetString(4);
                Console.WriteLine($"{PaymentID,5}|{GiverID,5}|{RecipientID,5}|£{Amount,5}|{Description,5}");

            }
            // get list of money given
            //sql = $"SELECT * FROM Payment WHERE GiverID={id}";*/


        } else
        {
            Console.WriteLine($"No user found: {username}");
        }
            c.Close();
    }
}
