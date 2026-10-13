using SemacV14.Service;

namespace Township_API.Services
{
    public static class ActorAsyncExtensions
    {
        // Replaces Actor.SendAndReceive() (which internally calls
        // Application.DoEvents() and requires System.Windows.Forms.dll)
        // with a purely event-driven, WinForms-free async wait.
        // Actor's own internal Timer (TimeoutMinisecond) already fires
        // OnEntityDataArrival with IsCompleted=false on timeout, so no
        // separate timeout logic is needed here.
        public static Task<ExecuteArgz> SendAndReceiveAsync(this Actor actor, uint timeoutMs = 10000)
        {
            var tcs = new TaskCompletionSource<ExecuteArgz>(TaskCreationOptions.RunContinuationsAsynchronously);

            void Handler(ExecuteArgz result)
            {
                actor.OnEntityDataArrival -= Handler;
                tcs.TrySetResult(result);
            }

            actor.OnEntityDataArrival += Handler;
            actor.TimeoutMinisecond = timeoutMs;
            actor.Send();

            return tcs.Task;
        }
    }
}