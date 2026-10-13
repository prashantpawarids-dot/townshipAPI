using System.Net.WebSockets;
using System.Text;

namespace Township_API.Services
{
    // Connects to the camera unit's own WebSocket (same one GateGuard uses),
    // sends start_stream, and relays every binary frame to the browser.
    // Runs only while a viewer is connected.
    public class FfmpegLiveViewHandler
    {
        private readonly IConfiguration _config;

        public FfmpegLiveViewHandler(IConfiguration config)
        {
            _config = config;
        }

        public async Task HandleAsync(WebSocket socket, string ip, CancellationToken ct)
        {
            var channel = _config["Camera:Channel"] ?? "1";
            var url = new Uri($"ws://{ip}/dws/video/{channel}/");

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            // Detect browser closing (nobody else reads from the browser socket)
            var clientWatcher = Task.Run(async () =>
            {
                var b = new byte[256];
                try
                {
                    while (!cts.IsCancellationRequested && socket.State == WebSocketState.Open)
                    {
                        var r = await socket.ReceiveAsync(b, cts.Token);
                        if (r.MessageType == WebSocketMessageType.Close) break;
                    }
                }
                catch { }
                finally { cts.Cancel(); }
            });

            var startMsg = Encoding.UTF8.GetBytes("{\"event_type\":\"start_stream\"}");
            var logPath = Path.Combine(AppContext.BaseDirectory, "camera_stream_debug.log");
            bool firstFrameLogged = false;

            while (!cts.IsCancellationRequested && socket.State == WebSocketState.Open)
            {
                using var cam = new ClientWebSocket();
                cam.Options.SetRequestHeader("Origin", $"http://{ip}");
                var cookie = _config["Camera:Cookie"];
                if (!string.IsNullOrEmpty(cookie))
                    cam.Options.SetRequestHeader("Cookie", cookie);

                try
                {
                    using (var connectCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token))
                    {
                        connectCts.CancelAfter(TimeSpan.FromSeconds(8));
                        await cam.ConnectAsync(url, connectCts.Token);
                    }

                    await cam.SendAsync(startMsg, WebSocketMessageType.Text, true, cts.Token);

                    var buf = new byte[64 * 1024];
                    using var ms = new MemoryStream();

                    while (!cts.IsCancellationRequested
                           && cam.State == WebSocketState.Open
                           && socket.State == WebSocketState.Open)
                    {
                        ms.SetLength(0);
                        WebSocketReceiveResult res;
                        do
                        {
                            res = await cam.ReceiveAsync(buf, cts.Token);
                            if (res.MessageType == WebSocketMessageType.Close) break;
                            ms.Write(buf, 0, res.Count);
                        } while (!res.EndOfMessage);

                        if (res.MessageType == WebSocketMessageType.Close) break;

                        if (res.MessageType == WebSocketMessageType.Text)
                        {
                            var text = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                            // Camera says stream ended -> ask it to start again
                            if (text.Contains("send_stream_end_message") || text.Contains("\"stream_status\": false"))
                                await cam.SendAsync(startMsg, WebSocketMessageType.Text, true, cts.Token);
                            continue;
                        }

                        if (!firstFrameLogged && ms.Length >= 8)
                        {
                            firstFrameLogged = true;
                            var head = BitConverter.ToString(ms.GetBuffer(), 0, 8);
                            try { File.WriteAllText(logPath, $"[{DateTime.Now}] first frame: {ms.Length} bytes, header {head}"); } catch { }
                        }

                        await socket.SendAsync(
                            new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length),
                            WebSocketMessageType.Binary, true, cts.Token);
                    }
                }
                catch (OperationCanceledException) { }
                catch (WebSocketException ex)
                {
                    try { File.AppendAllText(logPath, $"[{DateTime.Now}] camera ws error: {ex.Message}\n"); } catch { }
                }
                catch (Exception ex)
                {
                    try { File.AppendAllText(logPath, $"[{DateTime.Now}] error: {ex.Message}\n"); } catch { }
                }
                finally
                {
                    try
                    {
                        if (cam.State == WebSocketState.Open)
                            await cam.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    catch { }
                }

                if (!cts.IsCancellationRequested)
                    await Task.Delay(1500, CancellationToken.None); // short pause, then reconnect
            }

            cts.Cancel();
            await clientWatcher;
        }
    }
}