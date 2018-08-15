using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace SqliteConsoleTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var connection = new SqliteConnection("Data Source=:memory:"))
            {
                await connection.OpenAsync();

                var command = new SqliteCommand("SELECT 1;", connection);
                var result = (long) await command.ExecuteScalarAsync();

                Console.WriteLine($"Command output: {result}");
            }
        }
    }
}
