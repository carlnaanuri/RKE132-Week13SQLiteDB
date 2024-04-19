using System.Data.SQLite;

ReadData(CreateConnection());
//InsertCustomer(CreateConnection());
//RemoveCustomer(CreateConnection());
FindCustomer(CreateConnection());


static SQLiteConnection CreateConnection()
{
    SQLiteConnection connection = new SQLiteConnection("Data Source=mydb.db; Version = 3; New = True; Compress = True;");

    try
    {
        connection.Open();
        Console.WriteLine("DB found.");
    }
    catch
    {
        Console.WriteLine("DB not found.");
    }

    return connection;
}


static void ReadData(SQLiteConnection myConnection)
{
    Console.Clear();
    SQLiteDataReader reader;
    SQLiteCommand command;

    command = myConnection.CreateCommand();
    command.CommandText = "SELECT rowid, * FROM customer";


        /*"select customer.firstName, customer.lastName, status.statusType from customerStatus " +
        "join customer on customer.rowid = customerStatus.customerId " +
        "join status on status.rowid = customerStatus.statusId " +
        "ORDER BY status.StatusType";*/

    reader = command.ExecuteReader();

    while (reader.Read())
    {
        string readerRowId = reader["rowid"].ToString();
        string readerStringFirstName = reader.GetString(1);
        string readerStringLastName = reader.GetString(2);
        string readerStringDoB = reader.GetString(3);

        Console.WriteLine($"{readerRowId}. Full name: {readerStringFirstName} {readerStringLastName}; DoB: {readerStringDoB}");
    }

    myConnection.Close();
}


static void InsertCustomer(SQLiteConnection myConnection)
{
    SQLiteCommand command;
    string fName, lName, dob;     //salvestavad vahemälus endasse andmeid

    Console.WriteLine("Enter first name:");
    fName = Console.ReadLine();
    Console.WriteLine("Enter last name:");
    lName = Console.ReadLine();
    Console.WriteLine("Enter date of birth (mm-dd-yyyy):");
    dob = Console.ReadLine();

    command = myConnection.CreateCommand();
    command.CommandText = $"INSERT INTO customer(firstName, lastName, dateOfBirth) " +
    $"VALUES ('{fName}', '{lName}', '{dob}')";

    int rowInserted = command.ExecuteNonQuery();
    Console.WriteLine($"Row inserted: {rowInserted}");

  

    ReadData(myConnection);
}

static void RemoveCustomer(SQLiteConnection myConnection)
{
    SQLiteCommand command;

    string idToDelete;
    Console.WriteLine("Enter an id to delete a customer:");
    idToDelete = Console.ReadLine();

    command = myConnection.CreateCommand();
    command.CommandText = $"DELETE FROM customer WHERE rowid = {idToDelete}";
    int rowRemoved = command.ExecuteNonQuery();
    Console.WriteLine($"{rowRemoved} was removed from the table customer.");

    ReadData(myConnection);
}


static void FindCustomer(SQLiteConnection myConnection)
{
    SQLiteCommand command;
    string searchName;

    Console.WriteLine("Enter a first name to search for:");
    searchName = Console.ReadLine();

    // otsing
    command = myConnection.CreateCommand();
    command.CommandText = $"SELECT rowid, firstName, lastName, dateOfBirth FROM customer WHERE firstName LIKE @searchName";
    command.Parameters.AddWithValue("@searchName", $"%{searchName}%");

    
    using (SQLiteDataReader reader = command.ExecuteReader())
    {
        Console.WriteLine("Search results:");
        while (reader.Read())
        {
            int customerId = reader.GetInt32(0);
            string firstName = reader.GetString(1);
            string lastName = reader.GetString(2);
            string dateOfBirth = reader.GetString(3);

            Console.WriteLine($"ID: {customerId}, Name: {firstName} {lastName}, Date of Birth: {dateOfBirth}");
        }
    }
}