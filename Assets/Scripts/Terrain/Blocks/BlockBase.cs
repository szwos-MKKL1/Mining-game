using UnityEngine;

namespace Terrain.Blocks
{
    public class BlockBase : AbstractBlock
    {
        public override void OnBreak(BlockEventData blockEventData)
        {
            ///Debug.Log("Breaking block " + blockEventData.posInWorld);
        }
    }
}