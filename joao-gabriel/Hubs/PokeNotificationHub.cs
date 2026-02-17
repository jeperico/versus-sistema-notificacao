using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MinhaNotificacao.Hubs;

public class PokeNotificationHub : Hub
{
    // Armazena o mapeamento de UserId para ConnectionId
    private static readonly ConcurrentDictionary<int, HashSet<string>> UserConnections = new();

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
        {
            // Adicionar conexão para o usuário
            UserConnections.AddOrUpdate(
                userIdInt,
                new HashSet<string> { Context.ConnectionId },
                (key, existingSet) =>
                {
                    existingSet.Add(Context.ConnectionId);
                    return existingSet;
                });

            await Groups.AddToGroupAsync(Context.ConnectionId, $"User-{userIdInt}");
            
            var userName = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            Console.WriteLine($"[SignalR] Usuário {userName} (ID: {userIdInt}) conectado. ConnectionId: {Context.ConnectionId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
        {
            // Remover conexão do usuário
            if (UserConnections.TryGetValue(userIdInt, out var connections))
            {
                connections.Remove(Context.ConnectionId);
                
                if (connections.Count == 0)
                {
                    UserConnections.TryRemove(userIdInt, out _);
                }
            }

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User-{userIdInt}");
            
            var userName = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            Console.WriteLine($"[SignalR] Usuário {userName} (ID: {userIdInt}) desconectado. ConnectionId: {Context.ConnectionId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Métodos auxiliares estáticos para enviar notificações
    public static bool IsUserOnline(int userId)
    {
        return UserConnections.ContainsKey(userId) && UserConnections[userId].Count > 0;
    }

    public static int GetOnlineUsersCount()
    {
        return UserConnections.Count;
    }

    public static IEnumerable<int> GetOnlineUserIds()
    {
        return UserConnections.Keys;
    }
}
