using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace ScmDataAccess
{
    public interface IScmContext
    {
        IEnumerable<PartType> Parts { get; }

        IEnumerable<InventoryItem> Inventory { get; }

        IEnumerable<Supplier> Suppliers { get; }

        Task<PartCommand[]> GetPartCommands();

        Task DeletePartCommand(int id, DbTransaction transaction);

        Task UpdateInventoryItem(int partTypeId, int count, DbTransaction transaction);

        Task CreateOrder(Order order);

        DbTransaction BeginTransaction();

        Task<IEnumerable<Order>> GetOrders();
    }
}