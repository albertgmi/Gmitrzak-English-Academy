using System;
using System.Collections.Generic;
using System.Linq;
using inzBackend.Entities.Assignments;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.LiveNotepadModels;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.LiveNotepadServices
{
    public interface ILiveNotepadService
    {
        List<LiveNoteSummaryDto> GetLiveNotes(int? studentId = null);
        LiveNoteDetailDto GetLiveNoteById(int noteId);
        LiveNoteDetailDto CreateLiveNote(CreateLiveNoteRequest request);
        LiveNoteDetailDto SaveLiveNote(int noteId, SaveLiveNoteRequest request);
        void DeleteLiveNote(int noteId);
    }

    public class LiveNotepadService : ILiveNotepadService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public LiveNotepadService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public List<LiveNoteSummaryDto> GetLiveNotes(int? studentId = null)
        {
            var currentUserId = _userContextService.GetUserId!.Value;
            var role = _userContextService.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            var query = _dbContext.UserLiveNotes
                .Include(x => x.Student).ThenInclude(u => u.Profile)
                .AsQueryable();

            if (role != "Admin")
            {
                query = query.Where(x => x.StudentId == currentUserId);
            }
            else if (studentId.HasValue)
            {
                query = query.Where(x => x.StudentId == studentId.Value);
            }

            return query
                .OrderByDescending(x => x.LastModifiedAt)
                .Select(x => new LiveNoteSummaryDto
                {
                    Id = x.Id,
                    StudentId = x.StudentId,
                    StudentUsername = x.Student.Username,
                    StudentAvatarUrl = x.Student.Profile != null ? x.Student.Profile.AvatarUrl : null,
                    Title = string.IsNullOrWhiteSpace(x.Title) ? "Untitled Note" : x.Title,
                    PreviewText = StripHtml(x.Content, 100),
                    CreatedById = x.CreatedById,
                    CreatedByUsername = x.CreatedByUsername,
                    CreatedAt = x.CreatedAt,
                    LastModifiedAt = x.LastModifiedAt
                })
                .ToList();
        }

        public LiveNoteDetailDto GetLiveNoteById(int noteId)
        {
            var note = _dbContext.UserLiveNotes
                .Include(x => x.Student).ThenInclude(u => u.Profile)
                .FirstOrDefault(x => x.Id == noteId)
                ?? throw new NotFoundException($"Note {noteId} not found");

            var currentUserId = _userContextService.GetUserId!.Value;
            var role = _userContextService.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (role != "Admin" && note.StudentId != currentUserId)
            {
                throw new UnauthorizedAccessException("You do not have access to this note.");
            }

            return new LiveNoteDetailDto
            {
                Id = note.Id,
                StudentId = note.StudentId,
                StudentUsername = note.Student.Username,
                StudentAvatarUrl = note.Student.Profile != null ? note.Student.Profile.AvatarUrl : null,
                Title = string.IsNullOrWhiteSpace(note.Title) ? "Untitled Note" : note.Title,
                Content = note.Content,
                CreatedById = note.CreatedById,
                CreatedByUsername = note.CreatedByUsername,
                CreatedAt = note.CreatedAt,
                LastModifiedAt = note.LastModifiedAt
            };
        }

        public LiveNoteDetailDto CreateLiveNote(CreateLiveNoteRequest request)
        {
            var currentUserId = _userContextService.GetUserId!.Value;
            var currentUsername = _userContextService.GetUserName ?? "User";
            var role = _userContextService.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            int targetStudentId = currentUserId;

            if (role == "Admin" && request.StudentId.HasValue)
            {
                targetStudentId = request.StudentId.Value;
            }

            var student = _dbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefault(u => u.Id == targetStudentId)
                ?? throw new NotFoundException($"Student {targetStudentId} not found");

            var note = new UserLiveNote
            {
                StudentId = targetStudentId,
                Title = string.IsNullOrWhiteSpace(request.Title) ? "New Live Note" : request.Title.Trim(),
                Content = "<p>Start typing your shared notes here...</p>",
                CreatedById = currentUserId,
                CreatedByUsername = currentUsername,
                CreatedAt = PolandTime.DateTimeNow,
                LastModifiedAt = PolandTime.DateTimeNow
            };

            _dbContext.UserLiveNotes.Add(note);
            _dbContext.SaveChanges();

            return new LiveNoteDetailDto
            {
                Id = note.Id,
                StudentId = note.StudentId,
                StudentUsername = student.Username,
                StudentAvatarUrl = student.Profile?.AvatarUrl,
                Title = note.Title,
                Content = note.Content,
                CreatedById = note.CreatedById,
                CreatedByUsername = note.CreatedByUsername,
                CreatedAt = note.CreatedAt,
                LastModifiedAt = note.LastModifiedAt
            };
        }

        public LiveNoteDetailDto SaveLiveNote(int noteId, SaveLiveNoteRequest request)
        {
            var note = _dbContext.UserLiveNotes
                .Include(x => x.Student).ThenInclude(u => u.Profile)
                .FirstOrDefault(x => x.Id == noteId)
                ?? throw new NotFoundException($"Note {noteId} not found");

            var currentUserId = _userContextService.GetUserId!.Value;
            var role = _userContextService.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (role != "Admin" && note.StudentId != currentUserId)
            {
                throw new UnauthorizedAccessException("You do not have access to save this note.");
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                note.Title = request.Title.Trim();
            }

            note.Content = request.Content ?? string.Empty;
            note.LastModifiedAt = PolandTime.DateTimeNow;

            _dbContext.SaveChanges();

            return new LiveNoteDetailDto
            {
                Id = note.Id,
                StudentId = note.StudentId,
                StudentUsername = note.Student.Username,
                StudentAvatarUrl = note.Student.Profile?.AvatarUrl,
                Title = note.Title,
                Content = note.Content,
                CreatedById = note.CreatedById,
                CreatedByUsername = note.CreatedByUsername,
                CreatedAt = note.CreatedAt,
                LastModifiedAt = note.LastModifiedAt
            };
        }

        public void DeleteLiveNote(int noteId)
        {
            var role = _userContextService.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (role != "Admin")
            {
                throw new UnauthorizedAccessException("Only administrators can delete live notes.");
            }

            var note = _dbContext.UserLiveNotes
                .FirstOrDefault(x => x.Id == noteId)
                ?? throw new NotFoundException($"Note {noteId} not found");

            _dbContext.UserLiveNotes.Remove(note);
            _dbContext.SaveChanges();
        }

        private static string StripHtml(string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var plain = System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
            plain = System.Net.WebUtility.HtmlDecode(plain).Trim();
            if (plain.Length <= maxLength) return plain;
            return plain.Substring(0, maxLength) + "...";
        }
    }
}
