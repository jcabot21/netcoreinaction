using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
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

        private const string ConnectionStringKey = "ConnectionString";
        private const string DefaultConnectionString = "Data Source=:memory:";

        private static Dictionary<string, string> Config => new Dictionary<string, string>()
        {
            [ConnectionStringKey] = DefaultConnectionString
        };

        public IServiceProvider Services { get; }

        public SampleScmDataFixture()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.AddInMemoryCollection(Config)
                .AddJsonFile("config.json", true);
            
            var configRoot = configBuilder.Build();
            var connectionString = configRoot[ConnectionStringKey];
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient(provider =>
            {
                var connection = new SqliteConnection(connectionString);

                connection.Open();

                (new SqliteCommand(PartTypeTable, connection)).ExecuteNonQuery();

                return new SqliteScmContext(connection);
            });

            Services = serviceCollection.BuildServiceProvider();
        }
    }
}