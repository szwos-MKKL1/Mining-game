using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NativeTrees;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Terrain.Generator.Structure.Dungeon
{
    public class DungeonRoomTree<T> : IDisposable, IEnumerable<T> where T : unmanaged, IDungeonRoom
    {
        public NativeList<T> Rooms { get; }
        public NativeQuadtree<int> Tree { get; }

        public DungeonRoomTree(AABB2D bounds, Allocator allocator)
        {
            Rooms = new NativeList<T>(allocator);
            Tree = new NativeQuadtree<int>(bounds, allocator);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Rooms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public void Dispose()
        {
            Rooms.Dispose();
            Tree.Dispose();
        }

        //TODO
        // public JobHandle Dispose(JobHandle inputDeps)
        // {
        //     
        // }
    }
    
    public struct DungeonRoom : IDungeonRoom
    {
        public AABB2D Rect { get; set;}

        public float Area
        {
            get
            {
                float2 d = Rect.max - Rect.min;
                return d.x * d.y;
            }
        }

        public DungeonRoom(int2 pos, int2 size)
        {
            Rect = new AABB2D(pos, pos + size);
        }
    }

    public interface IDungeonRoom
    {
        public AABB2D Rect { get; set;}

        public float Area { get; }
    }
}