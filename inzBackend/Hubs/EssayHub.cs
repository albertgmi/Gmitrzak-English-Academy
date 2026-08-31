using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using inzBackend.Models.EssayModels;

namespace inzBackend.Hubs
{
    public class EssayHub : Hub
    {
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, CollaborativeUserDto>> _rooms 
            = new ConcurrentDictionary<int, ConcurrentDictionary<string, CollaborativeUserDto>>();

        public async Task JoinEssayRoom(int essayId, string username, string role)
        {
            var groupName = GetGroupName(essayId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var userDto = new CollaborativeUserDto
            {
                ConnectionId = Context.ConnectionId,
                Username = username,
                Role = role,
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

        public async Task SendLiveGrade(int essayId, int grammarScore, int vocabScore, int structureScore, string feedbackNotes)
        {
            var groupName = GetGroupName(essayId);
            await Clients.Group(groupName).SendAsync("ReceiveLiveGrade", new
            {
                GrammarScore = grammarScore,
                VocabScore = vocabScore,
                StructureScore = structureScore,
                FeedbackNotes = feedbackNotes,
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

        public async Task SendChatMessage(int essayId, string message, string senderUsername, string senderRole)
        {
            var groupName = GetGroupName(essayId);
            var chatMsg = new ChatMessageDto
            {
                SenderUsername = senderUsername,
                SenderRole = senderRole,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await Clients.Group(groupName).SendAsync("ReceiveChatMessage", chatMsg);
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
