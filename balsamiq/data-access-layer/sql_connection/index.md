# SqlConnection Class

`SqlConnection` is a class provided by the ADO.NET framework that represents a connection to a SQL Server database. It is used to establish a connection to the database, execute SQL commands, and retrieve data from the database.

The SqlConnection class is part of the System.Data.SqlClient namespace and is typically used in conjunction with other classes such as SqlCommand and SqlDataReader to interact with a SQL Server database.

Here's an example of how to use SqlConnection to establish a connection to a SQL Server database:

```csharp
using System.Data.SqlClient;

string connectionString = "Data Source=ServerName;Initial Catalog=DatabaseName;User ID=Username;Password=Password";

using (SqlConnection connection = new SqlConnection(connectionString))
{
    // Open the connection
    connection.Open();

    // Execute SQL commands or retrieve data from the database
}
```

The `SqlConnection` object is created within a `using` statement, which ensures that the connection is properly disposed of and resources are released when it goes out of scope. The `Open()` method is called to establish the connection to the database.

Once the connection is open, you can execute SQL commands by creating instances of the `SqlCommand` class and associating them with the `SqlConnection` object.

After you're done working with the database, the connection should be closed to release the resources using `Close()` method or enclose within `using` statement to close the connection automatically.
