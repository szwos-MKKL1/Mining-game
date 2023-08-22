namespace Terrain.Generator.Border
{
    public interface IBorderShape
    {
        bool IsInsideBorder(int posX, int posY);
        
    }
}