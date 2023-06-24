using System.Numerics;

namespace Terrain
{
    public interface IBorderShape
    {
        bool IsInsideBorder(int posX, int posY);
        
    }
}