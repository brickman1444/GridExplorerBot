namespace GridExplorerBot
{
    public class InventoryObject : DynamicObject
    {
        public override bool CanBePickedUp() { return true; }
    }
}