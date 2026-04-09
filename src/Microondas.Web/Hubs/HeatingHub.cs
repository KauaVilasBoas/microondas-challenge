using Microsoft.AspNetCore.SignalR;

namespace Microondas.Web.Hubs;

/// <summary>
/// SignalR hub that pushes real-time heating events to connected browsers.
/// The hub itself is empty — all push logic lives in <see cref="HeatingHubNotifier"/>,
/// which is injected into the MediatR event handlers.
/// </summary>
public sealed class HeatingHub : Hub
{
}