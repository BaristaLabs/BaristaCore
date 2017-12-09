namespace BaristaLabs.BaristaCore
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a simple implementation of a Promise Task Queue
    /// </summary>
    public class PromiseTaskQueue : IPromiseTaskQueue
    {
        private Queue<JsFunction> m_taskQueue = new Queue<JsFunction>();

        public int Count
        {
            get { return m_taskQueue.Count; }
        }

        public void Clear()
        {
            m_taskQueue.Clear();
        }

        public JsFunction Dequeue()
        {
            return m_taskQueue.Dequeue();
        }

        public void Enqueue(JsFunction promise)
        {
            m_taskQueue.Enqueue(promise);
        }
    }
}
