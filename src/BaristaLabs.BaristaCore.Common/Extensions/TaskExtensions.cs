namespace BaristaLabs.BaristaCore.Extensions
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains useful extension methods for tasks.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Returns a task that completes with the specified cancellation token requests cancellation.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task WhenCancelled(this CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }
    }
}
