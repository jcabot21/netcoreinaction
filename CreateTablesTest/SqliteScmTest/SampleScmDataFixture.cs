using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace SqliteScmTest
{
    public class SampleScmDataFixture : IDisposable
    {
        private const string PartTypeTable =
            @"CREATE TABLE PartType(
                    Id INTEGER PRIMARY KEY,
                    Name VARCHAR(255) NOT NULL
            );";
        
        private const string InventoryItemTable =
            @"CREATE TABLE InventoryItem(
                PartTypeId INTEGER PRIMARY KEY,
                Count INTEGER NOT NULL,
                OrderThreshold INTEGER,
            FOREIGN KEY(PartTypeId) REFERENCES PartType(Id)
            )";

        public SqliteConnection Connection { get; }

        public SampleScmDataFixture()
        {
            Connection = new SqliteConnection("Data Source=:memory:");

            Connection.Open();

            var command = new SqliteCommand(PartTypeTable, Connection);
            
            command.ExecuteNonQuery();

            command = new SqliteCommand(InventoryItemTable, Connection);
            
            command.ExecuteNonQuery();

            command = new SqliteCommand(
                "INSERT INTO PartType (Name) VALUES ('8289 L-shaped plate');",
                Connection);

            command.ExecuteNonQuery();

            command = new SqliteCommand(
                @"INSERT INTO InventoryItem
                    (PartTypeId, Count, OrderThreshold)
                    VALUES
                    (1, 100, 10)"
                , Connection);

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