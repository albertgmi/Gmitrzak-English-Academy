using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inzBackend.Models.LiveNotepadModels;
using Microsoft.AspNetCore.SignalR;

namespace inzBackend.Hubs
{
    public class LiveNotepadHub : Hub
    {
        private static readonly ConcurrentDictionary<string, List<LiveNoteCollaborativeUserDto>> RoomUsers = new();

        public async Task JoinNoteRoom(int noteId, string username, string role, string? avatarUrl)
        {
            var roomName = GetRoomName(noteId);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            var user = new LiveNoteCollaborativeUserDto
            {
                ConnectionId = Context.ConnectionId,
                Username = username,
                Role = role,
                AvatarUrl = avatarUrl,
                JoinedAt = DateTime.UtcNow
            };

            var users = RoomUsers.GetOrAdd(roomName, _ => new List<LiveNoteCollaborativeUserDto>());
            lock (users)
            {
                users.RemoveAll(u => u.ConnectionId == Context.ConnectionId);
                users.Add(user);
            }

            await Clients.Group(roomName).SendAsync("UserJoined", user);
            await Clients.Caller.SendAsync("ActiveUsersList", users.ToList());
        }

        public async Task LeaveNoteRoom(int noteId)
        {
            var roomName = GetRoomName(noteId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);

            if (RoomUsers.TryGetValue(roomName, out var users))
            {
                lock (users)
                {
                    users.RemoveAll(u => u.ConnectionId == Context.ConnectionId);
                }
                await Clients.Group(roomName).SendAsync("UserLeft", Context.ConnectionId);
            }
        }

        public async Task SendContentChange(int noteId, string content, string senderUsername)
        {
            var roomName = GetRoomName(noteId);
            await Clients.OthersInGroup(roomName).SendAsync("ContentChanged", new
            {
                noteId,
                content,
                senderUsername,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task SendSelectionChange(int noteId, int index, int length, string senderUsername, string role)
        {
            var roomName = GetRoomName(noteId);
            await Clients.OthersInGroup(roomName).SendAsync("SelectionChanged", new
            {
                noteId,
                index,
                length,
                username = senderUsername,
                role
            });
        }

        public async Task SendTypingStatus(int noteId, bool isTyping, string senderUsername)
        {
            var roomName = GetRoomName(noteId);
            await Clients.OthersInGroup(roomName).SendAsync("UserTyping", new
            {
                noteId,
                isTyping,
                senderUsername
            });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var kvp in RoomUsers)
            {
                var roomName = kvp.Key;
                var users = kvp.Value;
                bool removed;
                lock (users)
                {
                    removed = users.RemoveAll(u => u.ConnectionId == Context.ConnectionId) > 0;
                }

                if (removed)
                {
                    await Clients.Group(roomName).SendAsync("UserLeft", Context.ConnectionId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        private static string GetRoomName(int noteId) => $"live_note_{noteId}";
    }
}
