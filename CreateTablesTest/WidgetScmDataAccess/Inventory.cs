using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WidgetScmDataAccess
{
    public class Inventory
    {
        private readonly ScmContext _context;

        public Inventory(ScmContext context)
        {
            _context = context;
        }

        public async Task OrderPart(PartType part, int count)
        {
            var order = new Order()
            {
                PartTypeId = part.Id,
                PartCount = count,
                PlacedDate = DateTime.Now
            };

            order.Part = _context.Parts.Single(p => p.Id == order.PartTypeId);
            order.Supplier = _context.Suppliers.First(s => s.PartTypeId == part.Id);
            order.SupplierId = order.Supplier.Id;

            await _context.CreateOrder(order);
        }

        public async Task UpdateInventory()
        {
            foreach (var cmd in await _context.GetPartCommands())
            {
                var item = _context.Inventory.Single(i => i.PartTypeId == cmd.PartTypeId);
                var oldCount = item.Count;

                if (cmd.Command == PartCountOperation.Add)
                {
                    item.Count += cmd.PartCount;
                }
                else
                {
                    item.Count -= cmd.PartCount;
                }

                using (var transaction = _context.BeginTransaction())
                {
                    try
                    {
                        await _context.UpdateInventoryItem(item.PartTypeId, item.Count, transaction);
                        await _context.DeletePartCommand(cmd.Id, transaction);

                        transaction.Commit();
                    }
                    catch 
                    {
                        transaction.Rollback();
                        item.Count = oldCount;
                        throw;
                    }
                }
            }

            var orders = await _context.GetOrders();

            foreach (var item in _context.Inventory)
            {
                if (item.Count < item.OrderThreshold && 
                    orders.FirstOrDefault(o => o.PartTypeId == item.PartTypeId && !o.FulfilledDate.HasValue) == null)
                {
                    await OrderPart(item.PartType, item.OrderThreshold);
                }
            }
        }
    }
}