using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace WidgetScmDataAccess
{
    public class ScmContext
    {
        private readonly DbConnection _connection;
        private readonly Lazy<IEnumerable<PartType>> _parts;
        private readonly Lazy<IEnumerable<InventoryItem>> _inventory;
        private readonly Lazy<IEnumerable<PartCommand>> _commands;

        private readonly Lazy<IEnumerable<Supplier>> _suppliers;

        public IEnumerable<PartType> Parts => _parts.Value;
        public IEnumerable<InventoryItem> Inventory => _inventory.Value;
        public IEnumerable<PartCommand> Commands => _commands.Value;

        public IEnumerable<Supplier> Suppliers => _suppliers.Value;

        public ScmContext(DbConnection connection)
        {
            _connection = connection;
            _parts = new Lazy<IEnumerable<PartType>>(() => ReadParts().Result);
            _inventory = new Lazy<IEnumerable<InventoryItem>>(() => ReadInventory().Result);
            _commands = new Lazy<IEnumerable<PartCommand>>(() => GetPartCommands().Result);
            _suppliers = new Lazy<IEnumerable<Supplier>>(() => ReadSuppliers().Result);
        }

        public DbTransaction BeginTransaction() => _connection.BeginTransaction();

        public async Task CreateOrder(Order order)
        {
            using (var transaction = BeginTransaction())
            {
                try
                {
                    using (var orderCommand = _connection.CreateCommand())
                    using (var emailCommand = _connection.CreateCommand())
                    {
                        orderCommand.Transaction = transaction;
                        emailCommand.Transaction = transaction;

                        orderCommand.CommandText = 
                            @"INSERT INTO [Order]
                            (SupplierId, PartTypeId, PartCount, PlacedDate)
                            VALUES
                            (@supplierId, @partTypeId, @partCount, @placedDate);
                            SELECT last_insert_rowid();";
                        
                        AddParameter(orderCommand, "@supplierId", order.SupplierId);
                        AddParameter(orderCommand, "@partTypeId", order.PartTypeId);
                        AddParameter(orderCommand, "@partCount", order.PartCount);
                        AddParameter(orderCommand, "@placedDate", order.PlacedDate);

                        var orderId = (long) await orderCommand.ExecuteScalarAsync();

                        order.Id = (int) orderId;

                        emailCommand.CommandText = 
                            @"INSERT INTO SendEmailCommand
                            ([To], Subject, Body)
                            VALUES
                            (@to, @subject, @body)";
                        
                        AddParameter(emailCommand, "@to", order.Supplier.Email);
                        AddParameter(emailCommand, "@subject", $"ORder #{order.Id} for {order.Part.Name}");
                        AddParameter(emailCommand, "@body", $"Please send {order.PartCount} items of {order.Part.Name} to Widget Corp");

                        await emailCommand.ExecuteNonQueryAsync();

                        transaction.Commit();
                    }
                }
                catch 
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task UpdateInventoryItem(int partTypeId, int count, DbTransaction transaction)
        {
            using (var command = _connection.CreateCommand())
            {
                if (transaction != null) 
                {
                    command.Transaction = transaction;
                }

                command.CommandText = 
                    @"UPDATE InventoryItem
                    SET Count=@count
                    WHERE PartTypeId=@partTypeId";
                
                AddParameter(command, "@count", count);
                AddParameter(command, "@partTypeId", partTypeId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeletePartCommand(int id, DbTransaction transaction)
        {
            using (var command = _connection.CreateCommand())
            {
                if (transaction != null) 
                {
                    command.Transaction = transaction;
                }

                command.CommandText = "DELETE FROM PartCommand WHERE Id=@id";

                AddParameter(command, "@id", id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task CreatePartCommand(PartCommand partCommand)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = 
                    @"INSERT INTO PartCommand
                    (PartTypeId, Count, Command)
                    VALUES
                    (@partTypeId, @partCount, @command);
                    
                    SELECT last_insert_rowid();"; // Ensure we get the id for last insertion

                AddParameter(command, "@partTypeId", partCommand.PartTypeId);
                AddParameter(command, "@partCount", partCommand.PartCount);
                AddParameter(command, "@command", partCommand.Command.ToString());

                var partCommandId = (long) await command.ExecuteScalarAsync();

                partCommand.Id = (int) partCommandId;
            }
        }

        public async Task<IEnumerable<Supplier>> ReadSuppliers()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name, Email, PartTypeId FROM Supplier";

                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    var suppliers = new List<Supplier>();

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var supplier = new Supplier()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Email = reader.GetString(2),
                            PartTypeId = reader.GetInt32(3)
                        };

                        supplier.Part = Parts.Single(p => p.Id == supplier.PartTypeId);

                        suppliers.Add(supplier);
                    }

                    return suppliers;
                }
            }
        }

        public async Task<IEnumerable<PartCommand>> GetPartCommands()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = 
                    @"SELECT Id, PartTypeid, Count, Command
                    FROM PartCommand
                    ORDER BY Id";
                
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    var partCommands = new List<PartCommand>();

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var pc = new PartCommand()
                        {
                            Id = reader.GetInt32(0),
                            PartTypeId = reader.GetInt32(1),
                            PartCount = reader.GetInt32(2)
                        };

                        if (Enum.TryParse<PartCountOperation>(reader.GetString(3), out PartCountOperation operation))
                        {
                            pc.Command = operation;
                        }

                        pc.Part = Parts.Single(p => p.Id == pc.PartTypeId);

                        partCommands.Add(pc);
                    }

                    return partCommands;
                }
            }
        }

        private void AddParameter(DbCommand command, string name, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var p = command.CreateParameter();
            var type = value.GetType();

            if (type == typeof(int))
            {
                p.DbType = DbType.Int32;
            }
            else if (type == typeof(string))
            {
                p.DbType = DbType.String;
            }
            else if (type == typeof(DateTime))
            {
                p.DbType = DbType.DateTime;
            }
            else
            {
                throw new ArgumentException($"Unrecognized type: {type}", nameof(value));
            }

            p.Direction = ParameterDirection.Input;
            p.ParameterName = name;
            p.Value = value;

            command.Parameters.Add(p);
        }

        private async Task<IEnumerable<InventoryItem>> ReadInventory()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = 
                    @"SELECT PartTypeId, Count, OrderThreshold
                    FROM InventoryItem";

                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    var items = new List<InventoryItem>();

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var partId = reader.GetInt32(0);

                        items.Add(new InventoryItem()
                        {
                            PartTypeId = partId,
                            Count = reader.GetInt32(1),
                            OrderThreshold = reader.GetInt32(2),
                            PartType = Parts.Single(p => p.Id == partId)
                        });
                    }

                    return items;
                }
            }
        }

        private async Task<IEnumerable<PartType>> ReadParts()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name FROM PartType";

                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    var parts = new List<PartType>();

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        parts.Add(new PartType()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }

                    return parts;
                }
            }
        }
    }
}