using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace SqliteScmTest
{
    public class SampleScmDataFixture : IDisposable
    {
        public SqliteConnection Connection { get; }

        public SampleScmDataFixture()
        {
            Connection = new SqliteConnection("Data Source=:memory:");

            Connection.Open();

            var command = new SqliteCommand(
                @"CREATE TABLE PartType(
                    Id INTEGER PRIMARY KEY,
                    Name VARCHAR(255) NOT NULL
                );", Connection);
            
            command.ExecuteNonQuery();

            command = new SqliteCommand(
                "INSERT INTO PartType (Name) VALUES ('8289 L-shaped plate');",
                Connection);

            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            if (Connection !=null)
            {
                Connection.Dispose();
            }
        }
    }
}