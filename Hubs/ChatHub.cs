using Microsoft.AspNetCore.SignalR;

namespace API_WebH3.Hubs;

public class ChatHub : Hub
{
    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task LeaveChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task SendMessage(string chatId, object message)
    {
        await Clients.Group(chatId).SendAsync("ReceiveMessage", message);
    }
}