using NativeTrees;
using Random;
using Terrain.Blocks;
using Terrain.Generator.Structure.Dungeon;
using Terrain.Outputs;
using Unity.Mathematics;

namespace Terrain.Generator.Structure
{
    public class DungeonStructure : Structure
    {
        private BlockBase Wall = BlockRegistry.DUNGEONBLOCK;
        public DungeonStructure()
        {
            
        }

        public override BlockCollector getStructureBlocks(Context context)
        {
            IRandom random = context.Random;
            
            //TODO set position from context
            SimpleDungeonGen.Config config = new SimpleDungeonGen.Config(120,
                new BaseRandomRoomSize(new GaussianRandom(random, 0.2f, 0.5f), 20, 40, 0.2f), 
                new RandomPointCircle(random, new float2(500f, 500f), 50f));
            config.Bounds = new AABB2D(new float2(300, 300), new float2(700, 700));
            SimpleDungeonGen dungeonGenerator = new SimpleDungeonGen(config, random);
            
            dungeonGenerator.GeneratorOutput.Draw(30);
            CollectorBase<PosPair<DungeonRoom.DungeonBlockTypes>> dungeonCollector = new DungOutRasterisation(dungeonGenerator.GeneratorOutput).GetResult();
            BlockCollector blockCollector = new BlockCollector();
            foreach (PosPair<DungeonRoom.DungeonBlockTypes> dungeonBlock in dungeonCollector)
            {
                blockCollector.Add(new PosPair<BlockBase>(Wall, dungeonBlock.Pos));
            }

            dungeonGenerator.Dispose();
            return blockCollector;
        }
    }
}