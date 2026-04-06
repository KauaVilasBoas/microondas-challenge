using Microsoft.AspNetCore.SignalR;

namespace Microondas.Web.Hubs;

public sealed class HeatingHub : Hub
{
    public override Task OnConnectedAsync() => base.OnConnectedAsync();
    public override Task OnDisconnectedAsync(Exception? exception) => base.OnDisconnectedAsync(exception);
}
