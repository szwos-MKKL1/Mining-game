namespace Terrain.Blocks
{
    public interface IDropable
    {
        public void DropItems(BlockEventData blockEventData, RandomDropSettings randomDropSettings)
        {
            //TODO spawn items?
        }
    }
}