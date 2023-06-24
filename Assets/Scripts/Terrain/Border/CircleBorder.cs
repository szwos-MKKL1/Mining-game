using System.Numerics;

namespace Terrain
{
    //TODO add center of circle
    public class CircleBorder : IBorderShape
    {
        private readonly int sqRadius;
        public CircleBorder(float radius)
        {
            sqRadius = (int)(radius * radius);
        }
        
        public bool IsInsideBorder(int posX, int posY)
        {
            return posX * posX + posY * posY < sqRadius;
        }
    }
}