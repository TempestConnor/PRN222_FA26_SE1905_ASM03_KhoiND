using Microsoft.AspNetCore.SignalR.Client;
using HorseClub.Services.KhoiND.Realtime;

namespace HorseClub.BlazorWebApp.KhoiND.Services
{
    // Tells the hub host (when "HubUrl" is configured) about writes made in this app.
    // The hub only exposes Broadcast to clients, so the change goes out as a Notification.
    // Without a HubUrl, or if the hub is down, writes still succeed and nothing is sent.
    public class HorseHubNotifier : IAsyncDisposable
    {
        private readonly HubConnection? _hub;
        private readonly SemaphoreSlim _connectLock = new(1, 1);
        private readonly ILogger<HorseHubNotifier> _log;

        public HorseHubNotifier(IConfiguration config, ILogger<HorseHubNotifier> log)
        {
            _log = log;
            var hubUrl = config["HubUrl"];
            if (!string.IsNullOrWhiteSpace(hubUrl))
            {
                _hub = new HubConnectionBuilder().WithUrl(hubUrl).WithAutomaticReconnect().Build();
            }
        }

        public async Task BroadcastAsync(string message)
        {
            if (_hub is null) return;   // standalone mode

            try
            {
                await _connectLock.WaitAsync();
                try
                {
                    if (_hub.State == HubConnectionState.Disconnected)
                        await _hub.StartAsync();
                }
                finally
                {
                    _connectLock.Release();
                }

                await _hub.InvokeAsync(HubContract.Broadcast, message);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Could not broadcast to the hub");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_hub is not null) await _hub.DisposeAsync();
            _connectLock.Dispose();
        }
    }
}
