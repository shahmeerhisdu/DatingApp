using System.Collections.Concurrent;

namespace API.SignalR
{
    public class PresenceTracker
    {
        // The purpose of this class is to hold in memory the users and their connections to the presence hub.But this approch is not for multiple servers or scalable, because this will have the in memory for this particular server only.
        // if we had two instances of our API that were load balanced then some users would be connected to one instance of this some would be connected to the other and this simply would not work.
        // But we will look to the approach that is scalable.

        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> OnlineUsers = new(); // now the one is the string that will contain the userId but the user can have multiple connections, in different browsers in different laptop in mobile etc so we have the second parameter as the CuncurrentDictionary as well that will conatin the connectionId as the string and the next byte will not contain much useful information, but because we are using the concurrentDictionary it is mandatory to use this as the second parameter

        public Task UserConnected(string userId, string connectionId)
        {
            var connections = OnlineUsers.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
            connections.TryAdd(connectionId, 0);
            return Task.CompletedTask;
        }

        public Task UserDisconnected(string userId, string connectionId)
        {
            if (OnlineUsers.TryGetValue(userId, out var connections )) // out keyword will give us all the users for this userId
            {
                connections.TryRemove(connectionId, out _);
                if (connections.IsEmpty)
                {
                    //if there are no other connections remove the userId from the outer dictionary
                    OnlineUsers.TryRemove(userId, out _);
                }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            return Task.FromResult(OnlineUsers.Keys.OrderBy(k => k).ToArray());
        }
    }
}
