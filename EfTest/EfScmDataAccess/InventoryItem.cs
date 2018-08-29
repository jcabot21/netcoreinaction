namespace EfScmDataAccess
{
    public class InventoryItem
    {
        public int Id { get; set; }

        public int Count { get; set; }

        public int OrderThreshold { get; set; }

        public PartType PartType { get; set; }
    }
}