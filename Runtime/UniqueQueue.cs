using System.Collections.Generic;

namespace StateMachinePackage.Runtime
{
    public class UniqueQueue<T>
    {
        private readonly Queue<T> queue = new();
        private readonly HashSet<T> set = new();

        public int Count => queue.Count;

        public void Enqueue(T item)
        {
            if (set.Add(item))
                queue.Enqueue(item);
        }

        public T Dequeue()
        {
            T item = queue.Dequeue();
            set.Remove(item);
            return item;
        }

        public UniqueQueue<T> ShallowCopy()
        {
            UniqueQueue<T> clone = new();
            foreach (T item in queue)
                clone.Enqueue(item);

            return clone;
        }

        public void Clear()
        {
            queue.Clear();
            set.Clear();
        }
    }
}