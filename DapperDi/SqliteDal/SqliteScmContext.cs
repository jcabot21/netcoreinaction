using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using ScmDataAccess;

namespace SqliteDal 
{
    public class SqliteScmContext : IScmContext
    {
        private readonly SqliteConnection _connection;

        public IEnumerable<PartType> Parts { get; }

        public IEnumerable<InventoryItem> Inventory { get; }

        public IEnumerable<Supplier> Suppliers { get; }

        public SqliteScmContext(SqliteConnection connection)
        {
            _connection = connection;

            _connection.Open();

            Parts = _connection.Query<PartType>("SELECT * FROM PartType");
            Inventory = _connection.Query<InventoryItem>("SELECT * FROM InventoryItem");

            foreach (var item in Inventory)
            {
                item.PartType = Parts.Single(p => p.Id == item.PartTypeId);
            }

            Suppliers = _connection.Query<Supplier>("SELECT * FROM Supplier");

            foreach (var supplier in Suppliers)
            {
                supplier.Part = Parts.Single(p => p.Id == supplier.PartTypeId);
            }
        }

        public DbTransaction BeginTransaction() => _connection.BeginTransaction();

        public async Task CreateOrder(Order order)
        {
            using (var transaction = BeginTransaction())
            {
                try
                {
                    order.Id = (await _connection.QueryAsync<int>(
                        @"INSERT INTO [Order]
                        (SupplierId, PartTypeId, PartCount, PlacedDate)
                        VALUES
                        (@SupplierId, @PartTypeId, @PartCount, @PlacedDate);
                        SELECT last_insert_rowid();",
                        order,
                        transaction
                    )).First();

                    await _connection.ExecuteAsync(
                        @"INSERT INTO SendEmailCommand
                        ([To], Subject, Body)
                        VALUES
                        (@To, @Subject, @Body)",
                        new 
                        {
                            To = order.Supplier.Email,
                            Subject = $"Order #{order.Id} for {order.Part.Name}",
                            Body = $"Please send {order.PartCount} items of {order.Part.Name} to Widget Corp"
                        },
                        transaction
                    );

                    transaction.Commit();
                }
                catch 
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public Task DeletePartCommand(int id, DbTransaction transaction) =>
            _connection.ExecuteAsync(
                @"DELETE FROM PartCommands 
                WHERE Id=@Id",
                new { Id = id },
                transaction
            );

        public async Task<IEnumerable<Order>> GetOrders()
        {
            var orders = await _connection.QueryAsync<Order>("SELECT * FROM [Order]");

            foreach (var order in orders)
            {
                order.Part = Parts.Single(p => p.Id == order.PartTypeId);
                order.Supplier = Suppliers.Single(s => s.Id == order.SupplierId);
            }

            return orders;
        }

        public async Task<PartCommand[]> GetPartCommands() =>
            (await _connection.QueryAsync<PartCommand>("SELECT * FROM PartCommand")).ToArray();

        public Task UpdateInventoryItem(int partTypeId, int count, DbTransaction transaction) =>
            _connection.ExecuteAsync(
                @"UPDATE InventoryItem
                SET Count=@Count
                WHERE PartTypeId=@PartTypeId",
                new { Count = count, PartTypeId = partTypeId },
                transaction
            );
    }
}