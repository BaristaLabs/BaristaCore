namespace BaristaLabs.BaristaCore
{
    /// <summary>
    /// Represents an object that enqueues promises for future resolution.
    /// </summary>
    public interface IPromiseTaskQueue
    {
        int Count
        {
            get;
        }

        void Clear();

        void Enqueue(JsFunction promise);

        JsFunction Dequeue();
    }
}
