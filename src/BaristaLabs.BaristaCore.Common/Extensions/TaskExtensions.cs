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

        public static object GetTaskResult(this Task task, out bool hasResult)
        {
            var taskType = task.GetType();
            if (!typeof(Task<>).IsSubclassOfRawGeneric(taskType))
            {
                hasResult = false;
                return null;
            }
            
            hasResult = true;
            var resultProperty = taskType.GetProperty("Result");
            var result = resultProperty.GetValue(task);

            if (result is Task resultTask)
            {
                result = resultTask.GetTaskResult(out hasResult);
            }

            return result;
        }
    }
}
