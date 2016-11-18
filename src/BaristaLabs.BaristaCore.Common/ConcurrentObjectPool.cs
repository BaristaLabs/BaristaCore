namespace BaristaLabs.BaristaCore
{
    using System;
    using System.Collections.Concurrent;

    public class ConcurrentObjectPool<T>
        where T : class
    {
        private readonly BlockingCollection<T> m_pool;
        private readonly Func<T> m_objectGenerator;

        public ConcurrentObjectPool(Func<T> objectGenerator)
            : this(objectGenerator, Environment.ProcessorCount, 0)
        {
        }

        public ConcurrentObjectPool(Func<T> objectGenerator, int boundedCapacity, int initializeCount)
        {
            if (objectGenerator == null)
                throw new ArgumentNullException(nameof(objectGenerator));

            m_pool = new BlockingCollection<T>(new ConcurrentBag<T>(), boundedCapacity);
            m_objectGenerator = objectGenerator;

            if (initializeCount > 0)
            {
                for (int i = 0; i < initializeCount; i++)
                {
                    m_pool.Add(m_objectGenerator());
                }
            }
        }

        public T Get()
        {
            T item;
            if (m_pool.TryTake(out item))
                return item;

            return m_objectGenerator();
        }

        public void Return(T obj)
        {
            m_pool.Add(obj);
        }
    }
}
