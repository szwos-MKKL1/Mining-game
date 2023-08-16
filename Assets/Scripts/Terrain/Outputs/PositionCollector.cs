using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting.FullSerializer;

namespace Terrain.Outputs
{
    public struct PositionCollector<T> : IPositionCollector<T> where T : struct, IPositionHolder
    {
        //TODO position collector should be without offset, so no min and max, and rename to IPositionCollector to something with rect
        private NativeArray<T> collector;
        public int2 Min { get; }
        public int2 Max { get; }

        public PositionCollector(int2 min, int2 max, Allocator allocator)
        {
            Min = min;
            Max = max;
            int2 size = math.abs(max - min);
            collector = new NativeArray<T>(size.x * size.y, allocator);
        }
        
        public void Set(T value, int2 pos)
        {
            throw new System.NotImplementedException();
        }

        public T At(int2 pos)
        {
            return collector[pos]
        }

        public IEnumerator<ValuePosition<T>> GetEnumerator()
        {
            return new Enumerator(ref collector, Max.x, Min);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public struct Enumerator : IEnumerator<ValuePosition<T>>
        {
            private NativeArray<T> mArray;
            private int maxX;
            private int minX;
            private int mIndex;
            private int2 pos;

            public Enumerator(ref NativeArray<T> array, int maxX, int2 minPos)
            {
                this.mArray = array;
                this.mIndex = -1;
                this.maxX = maxX;
                this.pos = minPos - new int2(1, 0);
                this.minX = minPos.x;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                ++this.mIndex;
                if (pos.x < maxX)
                {
                    pos.x++;
                }
                else
                {
                    pos.x = minX;
                    pos.y++;
                }
                return this.mIndex < this.mArray.Length;
            }

            public void Reset()
            {
                this.mIndex = -1;
            }

            public ValuePosition<T> Current => new(this.mArray[this.mIndex], pos);

            object IEnumerator.Current => (object) this.Current;
        }
    }

    public interface IPositionCollector<T> : IEnumerable<ValuePosition<T>> where T : struct, IPositionHolder
    {
        public int2 Min { get; }
        public int2 Max { get; }
        public void Set(T value, int2 pos);
        //public void SetRange(T* values, int2* pos)//TODO not sure how to pass multiple values
        public T At(int2 pos);
    }

    public struct ValuePosition<T> where T : struct, IPositionHolder
    {
        public T Value;
        public int2 Pos;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValuePosition(T value, int2 pos)
        {
            Value = value;
            Pos = pos;
        }
    }
}