using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace WebAPITest.Hubs;

public class MessageHub : Hub
{
    private static readonly ConcurrentDictionary<string, string> ConnectedUsers = new();

    public async Task SendMessage(string recipientsUserId, string message)
    { 
        
        //await Clients.All.SendAsync("ReceiveMessage", _connectedUsers[Context.ConnectionId], message);
        if (FindConnectionIdByUserId(recipientsUserId, out var recipientsConnectionId))
        {
            await Clients.Client(recipientsConnectionId).SendAsync("ReceiveMessage",recipientsUserId, message);
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", ConnectedUsers[Context.ConnectionId], message);
            //add messageToDatabase
        }
        else
        {
            //add messageToDatabase
        }
        
    }

    public override async Task OnConnectedAsync()
    {
        var accessToken = Context.GetHttpContext()!.Request.Query["access_token"];
        if (accessToken.Count == 1)
        {
            ConnectedUsers[Context.ConnectionId] = accessToken;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
            ConnectedUsers.TryRemove(Context.ConnectionId, out _);
    }
    
    private bool FindConnectionIdByUserId(string userId, out string connectionId)
    {
        connectionId = "";
        
        foreach (var pair in ConnectedUsers)
        {
            if (pair.Value != userId) continue;
            connectionId = pair.Key;
            return true;
        }
        return false;
    }
}