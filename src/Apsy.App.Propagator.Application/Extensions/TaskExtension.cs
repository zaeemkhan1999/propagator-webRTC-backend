namespace Apsy.App.Propagator.Application.Extensions
{
    public static class TaskExtension
    {
        public static void Forget(this Task task)
        {
            if (!task.IsCompleted || task.IsFaulted)
            {
                _ = ForgetAwaited(task);
            }

            async static Task ForgetAwaited(Task task)
            {
                await task.ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            }
        }
    }
}
