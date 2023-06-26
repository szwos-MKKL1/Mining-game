
namespace Terrain.Blocks
{
    public class RockBlock : BlockBase, IDropable
    {

        public override void OnBreak(BlockEventData blockEventData)
        {
            base.OnBreak(blockEventData);
            ((IDropable)this).DropItems(blockEventData, new RandomDropSettings());
        }
    }
}