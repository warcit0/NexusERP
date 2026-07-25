using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace NexusERP.WebDashboard.Services;

public class NotificationService : IAsyncDisposable
{
    private HubConnection? _hubConnection;
    public event Action<string, string>? OnNotificationReceived;

    public async Task InitializeAsync(string token)
    {
        if (_hubConnection != null)
            return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5174/notificationhub", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token)!;
            })
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, string>("ReceiveNotification", (user, message) =>
        {
            OnNotificationReceived?.Invoke(user, message);
        });

        try
        {
            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error conectando a SignalR: {ex.Message}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
