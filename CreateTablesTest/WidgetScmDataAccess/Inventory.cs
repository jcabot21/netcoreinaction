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

        public async Task UpdateInventory()
        {
            foreach (var cmd in await _context.GetPartCommands())
            {
                var item = _context.Inventory.Single(i => i.PartTypeId == cmd.PartTypeId);

                if (cmd.Command == PartCountOperation.Add)
                {
                    item.Count += cmd.PartCount;
                }
                else
                {
                    item.Count -= cmd.PartCount;
                }

                await _context.UpdateInventoryItem(item.PartTypeId, item.Count);
                await _context.DeletePartCommand(cmd.Id);
            }
        }
    }
}