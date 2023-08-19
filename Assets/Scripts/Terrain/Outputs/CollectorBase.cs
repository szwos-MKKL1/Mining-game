using System.Collections;
using System.Collections.Generic;

namespace Terrain.Outputs
{
    public class CollectorBase<T> : IEnumerable<T>
    {
        protected readonly List<T> Collector;

        public CollectorBase()
        {
            Collector = new List<T>();
        }
        
        public CollectorBase(int initialCount)
        {
            Collector = new List<T>(initialCount);
        }

        public void Add(T collectable)
        {
            Collector.Add(collectable);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Collector.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}