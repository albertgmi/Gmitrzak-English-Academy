using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using inzBackend.Models.EssayModels;

namespace inzBackend.Hubs
{
    public class EssayHub : Hub
    {
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, CollaborativeUserDto>> _rooms 
            = new ConcurrentDictionary<int, ConcurrentDictionary<string, CollaborativeUserDto>>();

        public async Task JoinEssayRoom(int essayId, string username, string role, string? avatarUrl = null)
        {
            var groupName = GetGroupName(essayId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var userDto = new CollaborativeUserDto
            {
                ConnectionId = Context.ConnectionId,
                Username = username,
                Role = role,
                AvatarUrl = avatarUrl,
                JoinedAt = DateTime.UtcNow
            };

            var roomUsers = _rooms.GetOrAdd(essayId, _ => new ConcurrentDictionary<string, CollaborativeUserDto>());
            roomUsers[Context.ConnectionId] = userDto;

            var activeUsers = roomUsers.Values.ToList();

            await Clients.Group(groupName).SendAsync("ActiveUsersUpdated", activeUsers);
            await Clients.OthersInGroup(groupName).SendAsync("UserJoined", userDto);
        }

        public async Task LeaveEssayRoom(int essayId)
        {
            var groupName = GetGroupName(essayId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            if (_rooms.TryGetValue(essayId, out var roomUsers))
            {
                if (roomUsers.TryRemove(Context.ConnectionId, out var removedUser))
                {
                    var activeUsers = roomUsers.Values.ToList();
                    await Clients.Group(groupName).SendAsync("ActiveUsersUpdated", activeUsers);
                    await Clients.OthersInGroup(groupName).SendAsync("UserLeft", removedUser);
                }
            }
        }

        public async Task SendContentChange(int essayId, string content, string field, string senderUsername)
        {
            var groupName = GetGroupName(essayId);
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveContentChange", new
            {
                Content = content,
                Field = field,
                SenderUsername = senderUsername,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task SendSelectionChange(int essayId, int index, int length, string senderUsername, string senderRole)
        {
            var groupName = GetGroupName(essayId);
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveSelectionChange", new
            {
                Index = index,
                Length = length,
                SenderUsername = senderUsername,
                SenderRole = senderRole
            });
        }

        public async Task SendTeacherNote(int essayId, string noteId, string selectedText, string noteContent, string category, string author)
        {
            var groupName = GetGroupName(essayId);
            await Clients.Group(groupName).SendAsync("ReceiveTeacherNote", new
            {
                NoteId = noteId,
                SelectedText = selectedText,
                NoteContent = noteContent,
                Category = category,
                Author = author,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task SendTypingStatus(int essayId, bool isTyping, string senderUsername)
        {
            var groupName = GetGroupName(essayId);
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveTypingStatus", new
            {
                IsTyping = isTyping,
                SenderUsername = senderUsername
            });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var roomKv in _rooms)
            {
                var essayId = roomKv.Key;
                var roomUsers = roomKv.Value;

                if (roomUsers.TryRemove(Context.ConnectionId, out var removedUser))
                {
                    var groupName = GetGroupName(essayId);
                    var activeUsers = roomUsers.Values.ToList();
                    await Clients.Group(groupName).SendAsync("ActiveUsersUpdated", activeUsers);
                    await Clients.OthersInGroup(groupName).SendAsync("UserLeft", removedUser);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        private static string GetGroupName(int essayId) => $"essay_{essayId}";
    }
}
