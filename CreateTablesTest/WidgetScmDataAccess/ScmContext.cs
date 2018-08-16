using System;
using System.Collections.Generic;
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

        public IEnumerable<PartType> Parts => _parts.Value;
        public IEnumerable<InventoryItem> Inventory => _inventory.Value;

        public ScmContext(DbConnection connection)
        {
            _connection = connection;
            _parts = new Lazy<IEnumerable<PartType>>(() => ReadParts().Result);
            _inventory = new Lazy<IEnumerable<InventoryItem>>(() => ReadInventory().Result);
        }

        private async Task<IEnumerable<InventoryItem>> ReadInventory()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = 
                    @"SELECT PartTypeId, Count, OrderThreshold
                    FROM InventoryItem";

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var items = new List<InventoryItem>();

                    while (await reader.ReadAsync())
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