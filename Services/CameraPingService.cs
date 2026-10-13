using System.Net.Sockets;

namespace Township_API.Services
{
    public class CameraCheckResult
    {
        public bool Connected { get; set; }
        public string? Message { get; set; }
    }

    public interface ICameraPingService
    {
        Task<CameraCheckResult> CheckAsync(string ip, int port, CancellationToken ct);
    }

    // Runs ONLY when user selects a camera on UI. No timer, no background loop.
    public class CameraPingService : ICameraPingService
    {
        private const int TimeoutMs = 2500;

        public async Task<CameraCheckResult> CheckAsync(string ip, int port, CancellationToken ct)
        {
            using var client = new TcpClient();
            try
            {
                var connectTask = client.ConnectAsync(ip, port);
                var timeoutTask = Task.Delay(TimeoutMs, ct);
                var completed = await Task.WhenAny(connectTask, timeoutTask);

                if (completed == timeoutTask || !client.Connected)
                    return new CameraCheckResult { Connected = false, Message = "Timed out / unreachable" };

                return new CameraCheckResult { Connected = true, Message = "OK" };
            }
            catch (Exception ex)
            {
                return new CameraCheckResult { Connected = false, Message = ex.Message };
            }
        }
    }
}