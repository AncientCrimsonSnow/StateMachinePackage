using System.Collections.Generic;

namespace StateMachinePackage.Runtime
{
    public class UniqueQueue<T>
    {
        private Queue<T> queue = new Queue<T>();
        private HashSet<T> set = new HashSet<T>();

        public void Enqueue(T item)
        {
            if (set.Add(item))
            {
                queue.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            T item = queue.Dequeue();
            set.Remove(item);
            return item;
        }

        public int Count
        {
            get { return queue.Count; }
        }
    }
}