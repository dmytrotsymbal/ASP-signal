using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace PeopleApi.Hubs;

public class PersonHub : Hub
{
    // словарь который хранит подписки клиентов
    private static readonly ConcurrentDictionary<string, List<string>> _subscriptions = new();


    // функция подпсики на человека
    public async Task SubscribeToPerson(string firstName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, firstName.ToLower()); // имя в качестве группы

        _subscriptions.AddOrUpdate(
            firstName.ToLower(),
            new List<string> { Context.ConnectionId },
            (key, oldValue) =>
            {
                oldValue.Add(Context.ConnectionId);
                return oldValue;
            });
    }

    public async Task UnsubscribeFromPerson(string firstName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, firstName.ToLower());

        if (_subscriptions.TryGetValue(firstName.ToLower(), out var connectionIds))
        {
            connectionIds.Remove(Context.ConnectionId);
            if (connectionIds.Count == 0)
            {
                _subscriptions.TryRemove(firstName.ToLower(), out _);
            }
        }
    }



    // удаляем ConnectionId из всех подписок при отключении клиента
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var subscription in _subscriptions)
        {
            subscription.Value.Remove(Context.ConnectionId);
            if (subscription.Value.Count == 0)
            {
                _subscriptions.TryRemove(subscription.Key, out _);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }



    // Метод для получения списка ConnectionId для firstName (используется в контроллере)
    public static List<string> GetConnectionsForPerson(string firstName)
    {
        return _subscriptions.TryGetValue(firstName.ToLower(), out var connectionIds)
            ? connectionIds
            : new List<string>();
    }
}