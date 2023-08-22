using NativeTrees;
using Random;
using Terrain.Blocks;
using Terrain.Outputs;
using Unity.Mathematics;

namespace Terrain.Generator.Structure.Dungeon
{
    public class DungeonStructure : Structure
    {
        private readonly BlockBase wall = BlockRegistry.DUNGEONBLOCK;
        private BlockBase wallHallway = BlockRegistry.DUNGEONHALLWAY;
        private BlockBase air = BlockRegistry.AIR;
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
                BlockBase blockBase;
                switch (dungeonBlock.Value)
                {
                    case DungeonRoom.DungeonBlockTypes.AIR:
                        blockBase = air;
                        break;
                    case DungeonRoom.DungeonBlockTypes.MAIN_ROOM_WALL:
                        blockBase = wall;
                        break;
                    case DungeonRoom.DungeonBlockTypes.STANDARD_ROOM_WALL:
                        blockBase = wall;
                        break;
                    case DungeonRoom.DungeonBlockTypes.HALLWAY_WALL:
                        blockBase = wallHallway;
                        break;
                    case DungeonRoom.DungeonBlockTypes.NONE:
                    default:
                        continue;
                }
                blockCollector.Add(new PosPair<BlockBase>(blockBase, dungeonBlock.Pos));
            }

            dungeonGenerator.Dispose();
            return blockCollector;
        }
    }
}