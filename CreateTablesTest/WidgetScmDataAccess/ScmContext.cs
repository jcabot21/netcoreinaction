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

        public IEnumerable<PartType> Parts => _parts.Value;
        public IEnumerable<InventoryItem> Inventory => _inventory.Value;
        public IEnumerable<PartCommand> Commands => _commands.Value;

        public ScmContext(DbConnection connection)
        {
            _connection = connection;
            _parts = new Lazy<IEnumerable<PartType>>(() => ReadParts().Result);
            _inventory = new Lazy<IEnumerable<InventoryItem>>(() => ReadInventory().Result);
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
                            ParTypeId = partId,
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