using System;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using ScmDataAccess;
using SqliteDal;

namespace SqliteScmTest
{
    public class SampleScmDataFixture
    {
        private const string PartTypeTable = 
            @"CREATE TABLE PartType(
                Id INTEGER PRIMARY KEY,
                Name VARCHAR(255) NOT NULL
            );";

        public IServiceProvider Services { get; }

        public SampleScmDataFixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient(provider =>
            {
                var connection = new SqliteConnection("Data Source=:memory:");

                connection.Open();

                (new SqliteCommand(PartTypeTable, connection)).ExecuteNonQuery();

                return new SqliteScmContext(connection);
            });

            Services = serviceCollection.BuildServiceProvider();
        }
    }
}