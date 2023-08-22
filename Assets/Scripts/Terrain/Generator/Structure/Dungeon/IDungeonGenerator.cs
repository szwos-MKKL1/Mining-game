namespace Terrain.Generator.Structure.Dungeon
{
    public interface IDungeonGenerator
    {
        DungeonOutput<DungeonRoom> GeneratorOutput { get; }
    }
}