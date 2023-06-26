using Terrain.Blocks;
using UnityEngine.Tilemaps;

namespace Terrain
{
    /**
     * Stores all essential data used for terrain generation
     */
    public class GenerationData
    {
        public int SizeX = 512;
        public int SizeY = 512;
        public IBorderShape BorderShape = new CircleBorder(200);
        public AbstractBlock AbstractBlock;
    }
}