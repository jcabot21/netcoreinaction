namespace WidgetScmDataAccess
{
    public class InventoryItem
    {
        public int ParTypeId { get; set; }

        public int Count { get; set; }

        public int OrderThreshold { get; set; }

        public PartType PartType { get; set; }
    }
}