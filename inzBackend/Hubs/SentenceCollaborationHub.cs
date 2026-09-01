using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using inzBackend.Models.SentenceModels;
using inzBackend.Helpers;

namespace inzBackend.Hubs
{
    public class SentenceCollaborationHub : Hub
    {
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, SentenceCollaborativeUserDto>> _rooms 
            = new ConcurrentDictionary<int, ConcurrentDictionary<string, SentenceCollaborativeUserDto>>();

        public async Task JoinSentenceRoom(int answerId, string username, string role, string? avatarUrl = null)
        {
            var groupName = GetGroupName(answerId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var userDto = new SentenceCollaborativeUserDto
            {
                ConnectionId = Context.ConnectionId,
                Username = username,
                Role = role,
                AvatarUrl = avatarUrl,
                JoinedAt = PolandTime.DateTimeNow
            };

            var roomUsers = _rooms.GetOrAdd(answerId, _ => new ConcurrentDictionary<string, SentenceCollaborativeUserDto>());
            roomUsers[Context.ConnectionId] = userDto;

            var activeUsers = roomUsers.Values.ToList();

            await Clients.Group(groupName).SendAsync("ActiveUsersUpdated", activeUsers);
            await Clients.OthersInGroup(groupName).SendAsync("UserJoined", userDto);
        }

        public async Task LeaveSentenceRoom(int answerId)
        {
            var groupName = GetGroupName(answerId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            if (_rooms.TryGetValue(answerId, out var roomUsers))
            {
                if (roomUsers.TryRemove(Context.ConnectionId, out var removedUser))
                {
                    var activeUsers = roomUsers.Values.ToList();
                    await Clients.Group(groupName).SendAsync("ActiveUsersUpdated", activeUsers);
                    await Clients.OthersInGroup(groupName).SendAsync("UserLeft", removedUser);
                }
            }
        }

        public async Task SendContentChange(int answerId, string content, string field, string senderUsername)
        {
            var groupName = GetGroupName(answerId);
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveContentChange", new
            {
                Content = content,
                Field = field,
                SenderUsername = senderUsername,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task SendSelectionChange(int answerId, int index, int length, string senderUsername, string senderRole)
        {
            var groupName = GetGroupName(answerId);
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveSelectionChange", new
            {
                Index = index,
                Length = length,
                SenderUsername = senderUsername,
                SenderRole = senderRole
            });
        }

        public async Task SendTeacherNote(int answerId, string noteId, string selectedText, string noteContent, string category, string author)
        {
            var groupName = GetGroupName(answerId);
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveTeacherNote", new
            {
                NoteId = noteId,
                SelectedText = selectedText,
                NoteContent = noteContent,
                Category = category,
                Author = author,
                Timestamp = PolandTime.DateTimeNow
            });
        }

        public async Task SendTypingStatus(int answerId, bool isTyping, string senderUsername)
        {
            var groupName = GetGroupName(answerId);
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
                var answerId = roomKv.Key;
                var roomUsers = roomKv.Value;

                if (roomUsers.TryRemove(Context.ConnectionId, out var removedUser))
                {
                    var groupName = GetGroupName(answerId);
                    var activeUsers = roomUsers.Values.ToList();
                    await Clients.Group(groupName).SendAsync("ActiveUsersUpdated", activeUsers);
                    await Clients.OthersInGroup(groupName).SendAsync("UserLeft", removedUser);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        private static string GetGroupName(int answerId) => $"sentence_ans_{answerId}";
    }
}
