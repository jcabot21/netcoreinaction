using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace WidgetScmDataAccess
{
    public class ScmContext
    {
        private readonly DbConnection _connection;
        private readonly Lazy<IEnumerable<PartType>> _parts;

        public IEnumerable<PartType> Parts => _parts.Value;

        public ScmContext(DbConnection connection)
        {
            _connection = connection;
            _parts = new Lazy<IEnumerable<PartType>>(() => ReadParts().Result);
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