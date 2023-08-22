using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;

namespace DataStructures
{
    public class BoundedQueue<T> : IEnumerable, INotifyCollectionChanged
    {
        //TODO: could use CircularFifoQueue (memory continous array)
        private Queue<T> queue;
        public readonly int size;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public BoundedQueue(int size) 
        { 
            this.size = size;
        }

        public void Enqueue(T obj)
        {
            if(queue.Count < size)
            {
                queue.Enqueue(obj);
            } else
            {
                queue.Enqueue(obj);
                queue.Dequeue(); //last item
            }
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
        }

        public void Remove(T obj)
        {
            queue = new Queue<T>(queue.Where(x => !x.Equals(obj)));
            CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove));
        }

        public IEnumerator GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        public bool Contains(T obj)
        {
            return queue.Contains(obj);
        }


    }

}

