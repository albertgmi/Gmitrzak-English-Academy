using inzBackend.Entities.LearningMaterials;
using inzBackend.Entities.Resources;
using inzBackend.Entities.SpacedRepetition;
using inzBackend.Entities;
using inzBackend.Enums;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models.AiSpellCheckingModels;
using inzBackend.Models.WordfinderModels;
using inzBackend.Models;
using inzBackend.Services.AiIntegrationServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.WordfinderServices
{
    public class WordfinderService : IWordfinderService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IAiTranslationService _aiTranslationService;
        private readonly IAiSpellCheckService _aiSpellCheckService;

        public WordfinderService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IAiTranslationService aiTranslationService,
            IAiSpellCheckService aiSpellCheckService)
        {
            _dbContext = dbContext;
            _aiTranslationService = aiTranslationService;
            _aiSpellCheckService = aiSpellCheckService;
        }

        public async Task<WordfinderCatalogueDto> CreateDraftAsync(int studentUserId, CreateWordfinderCatalogueDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Catalogue name cannot be empty");

            var userExists = await _dbContext.Users.AnyAsync(u => u.Id == studentUserId);
            if (!userExists)
                throw new NotFoundException("Student user not found");

            var wordfinder = new WordfinderCatalogue
            {
                StudentUserId = studentUserId,
                Name = dto.Name.Trim(),
                Status = WordfinderCatalogueStatus.Draft,
                CreatedAt = PolandTime.DateTimeNow
            };

            if (dto.Entries != null && dto.Entries.Any())
            {
                var validEntries = dto.Entries
                    .Where(e => !string.IsNullOrWhiteSpace(e.Front))
                    .Select(e => new WordfinderCatalogueEntry
                    {
                        Front = e.Front.Trim(),
                        Back = (e.Back ?? string.Empty).Trim()
                    })
                    .ToList();

                wordfinder.Entries = validEntries;
            }

            _dbContext.WordfinderCatalogues.Add(wordfinder);
            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(wordfinder.Id, studentUserId, isAdmin: false);
        }

        public async Task<List<WordfinderCatalogueListDto>> GetMyCataloguesAsync(int studentUserId)
        {
            var catalogues = await _dbContext.WordfinderCatalogues
                .Include(w => w.StudentUser)
                    .ThenInclude(u => u.Profile)
                .Include(w => w.Entries)
                .Where(w => w.StudentUserId == studentUserId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return catalogues.Select(MapToListDto).ToList();
        }

        public async Task<WordfinderCatalogueDto> GetByIdAsync(int id, int userId, bool isAdmin)
        {
            var wordfinder = await _dbContext.WordfinderCatalogues
                .Include(w => w.StudentUser)
                    .ThenInclude(u => u.Profile)
                .Include(w => w.Entries)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (wordfinder is null)
                throw new NotFoundException($"Wordfinder catalogue with Id {id} was not found");

            if (!isAdmin && wordfinder.StudentUserId != userId)
                throw new ForbiddenException("You do not have access to this Wordfinder catalogue");

            return MapToDto(wordfinder);
        }

        public async Task<WordfinderCatalogueDto> UpdateDraftAsync(int id, int studentUserId, UpdateWordfinderCatalogueDto dto)
        {
            var wordfinder = await _dbContext.WordfinderCatalogues
                .Include(w => w.Entries)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (wordfinder is null)
                throw new NotFoundException($"Wordfinder catalogue with Id {id} was not found");

            if (wordfinder.StudentUserId != studentUserId)
                throw new ForbiddenException("You do not have access to update this catalogue");

            if (wordfinder.Status == WordfinderCatalogueStatus.Approved)
                throw new BadRequestException("Approved catalogues cannot be modified");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Catalogue name cannot be empty");

            wordfinder.Name = dto.Name.Trim();

            // Clear existing entries and replace
            _dbContext.WordfinderCatalogueEntries.RemoveRange(wordfinder.Entries);

            if (dto.Entries != null && dto.Entries.Any())
            {
                var newEntries = dto.Entries
                    .Where(e => !string.IsNullOrWhiteSpace(e.Front))
                    .Select(e => new WordfinderCatalogueEntry
                    {
                        WordfinderCatalogueId = wordfinder.Id,
                        Front = e.Front.Trim(),
                        Back = (e.Back ?? string.Empty).Trim()
                    })
                    .ToList();

                _dbContext.WordfinderCatalogueEntries.AddRange(newEntries);
            }

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(wordfinder.Id, studentUserId, isAdmin: false);
        }

        public async Task DeleteDraftAsync(int id, int studentUserId)
        {
            var wordfinder = await _dbContext.WordfinderCatalogues
                .Include(w => w.Entries)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (wordfinder is null)
                throw new NotFoundException($"Wordfinder catalogue with Id {id} was not found");

            if (wordfinder.StudentUserId != studentUserId)
                throw new ForbiddenException("You do not have access to delete this catalogue");

            if (wordfinder.Status == WordfinderCatalogueStatus.Approved)
                throw new BadRequestException("Approved catalogues cannot be deleted");

            _dbContext.WordfinderCatalogueEntries.RemoveRange(wordfinder.Entries);
            _dbContext.WordfinderCatalogues.Remove(wordfinder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<WordfinderCatalogueDto> SubmitAsync(int id, int studentUserId)
        {
            var wordfinder = await _dbContext.WordfinderCatalogues
                .Include(w => w.Entries)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (wordfinder is null)
                throw new NotFoundException($"Wordfinder catalogue with Id {id} was not found");

            if (wordfinder.StudentUserId != studentUserId)
                throw new ForbiddenException("You do not have access to submit this catalogue");

            if (wordfinder.Status == WordfinderCatalogueStatus.Approved)
                throw new BadRequestException("Approved catalogues cannot be submitted for review");

            if (!wordfinder.Entries.Any(e => !string.IsNullOrWhiteSpace(e.Front)))
                throw new BadRequestException("Cannot submit an empty catalogue. Please add at least one vocabulary entry.");

            wordfinder.Status = WordfinderCatalogueStatus.PendingApproval;
            wordfinder.SubmittedAt = PolandTime.DateTimeNow;
            wordfinder.RejectionReason = null;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(wordfinder.Id, studentUserId, isAdmin: false);
        }

        public async Task<TranslateWordfinderEntryResponse> TranslateEntryAsync(TranslateWordfinderEntryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FrontText))
                return new TranslateWordfinderEntryResponse { TranslatedText = string.Empty };

            var translatedList = await _aiTranslationService.TranslateBatchAsync(
                new List<string> { request.FrontText.Trim() },
                request.TargetLanguage ?? "Polish");

            return new TranslateWordfinderEntryResponse
            {
                TranslatedText = translatedList.FirstOrDefault() ?? string.Empty
            };
        }

        public async Task<SpellCheckResult> SpellCheckEntryAsync(SpellCheckRequest request)
        {
            return await _aiSpellCheckService.CheckTextAsync(request.Text, request.Language ?? "English");
        }

        public async Task<List<WordfinderCatalogueListDto>> GetPendingCataloguesAsync(WordfinderCatalogueStatus? statusFilter = null)
        {
            var query = _dbContext.WordfinderCatalogues
                .Include(w => w.StudentUser)
                    .ThenInclude(u => u.Profile)
                .Include(w => w.Entries)
                .AsQueryable();

            if (statusFilter.HasValue)
            {
                query = query.Where(w => w.Status == statusFilter.Value);
            }

            var catalogues = await query
                .OrderByDescending(w => w.SubmittedAt ?? w.CreatedAt)
                .ToListAsync();

            return catalogues.Select(MapToListDto).ToList();
        }

        public async Task<WordfinderCatalogueDto> ApproveCatalogueAsync(int id, int adminUserId, ApproveWordfinderCatalogueRequest? request)
        {
            var wordfinder = await _dbContext.WordfinderCatalogues
                .Include(w => w.StudentUser)
                    .ThenInclude(u => u.Profile)
                .Include(w => w.Entries)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (wordfinder is null)
                throw new NotFoundException($"Wordfinder catalogue with Id {id} was not found");

            if (wordfinder.Status != WordfinderCatalogueStatus.PendingApproval)
                throw new BadRequestException("Only catalogues in PendingApproval status can be approved");

            // Apply any Admin modifications to entries before approving
            if (request?.ModifiedEntries != null)
            {
                _dbContext.WordfinderCatalogueEntries.RemoveRange(wordfinder.Entries);
                var newEntries = request.ModifiedEntries
                    .Where(e => !string.IsNullOrWhiteSpace(e.Front))
                    .Select(e => new WordfinderCatalogueEntry
                    {
                        WordfinderCatalogueId = wordfinder.Id,
                        Front = e.Front.Trim(),
                        Back = (e.Back ?? string.Empty).Trim()
                    })
                    .ToList();

                _dbContext.WordfinderCatalogueEntries.AddRange(newEntries);
                wordfinder.Entries = newEntries;
                await _dbContext.SaveChangesAsync();
            }

            var username = wordfinder.StudentUser?.Username ?? "Student";
            var initials = ComputeInitials(username);
            var finalCatalogueName = $"{initials}_{wordfinder.Name.Trim()}";

            // 1. Create System Catalogue
            var systemCatalogue = new Catalogue
            {
                Name = finalCatalogueName,
                UploadedDate = PolandTime.Today,
                UploadedByUserId = adminUserId
            };

            _dbContext.Catalogues.Add(systemCatalogue);
            await _dbContext.SaveChangesAsync();

            // 2. Create System CatalogueEntries
            var catEntries = wordfinder.Entries
                .Where(e => !string.IsNullOrWhiteSpace(e.Front))
                .Select(e => new CatalogueEntry
                {
                    CatalogueId = systemCatalogue.Id,
                    EntryDate = PolandTime.Today,
                    UserRef = username,
                    Entry = e.Front.Trim(),
                    TranslatedEntry = e.Back.Trim(),
                    ComputedKey = $"{initials}_{e.Front.Trim()}"
                })
                .ToList();

            _dbContext.CatalogueEntries.AddRange(catEntries);

            // 3. Create Global Vocabulary items with Category = finalCatalogueName
            var distinctVocabEntries = wordfinder.Entries
                .Where(e => !string.IsNullOrWhiteSpace(e.Front))
                .GroupBy(e => e.Front.Trim().ToLower())
                .Select(g => g.First())
                .ToList();

            var frontListToCompare = distinctVocabEntries.Select(v => v.Front.Trim().ToLower()).ToList();

            var existingVocabFronts = await _dbContext.Vocabulary
                .Where(v => frontListToCompare.Contains(v.Front.ToLower()))
                .Select(v => v.Front.ToLower())
                .ToListAsync();

            var newVocabToInsert = distinctVocabEntries
                .Where(v => !existingVocabFronts.Contains(v.Front.Trim().ToLower()))
                .Select(v => new Vocabulary
                {
                    Front = v.Front.Trim(),
                    Back = v.Back.Trim(),
                    Category = finalCatalogueName,
                    CatalogueId = systemCatalogue.Id
                })
                .ToList();

            if (newVocabToInsert.Any())
                _dbContext.Vocabulary.AddRange(newVocabToInsert);

            await _dbContext.SaveChangesAsync();

            // 4. Automatically assign Vocabulary as Flashcards for the student
            var vocabIdsForCatalogue = await _dbContext.Vocabulary
                .Where(v => v.CatalogueId == systemCatalogue.Id)
                .Select(v => v.Id)
                .ToListAsync();

            if (vocabIdsForCatalogue.Any())
            {
                var existingFlashcardVocabIds = await _dbContext.Flashcards
                    .Where(f => f.UserId == wordfinder.StudentUserId && vocabIdsForCatalogue.Contains(f.VocabularyId))
                    .Select(f => f.VocabularyId)
                    .ToListAsync();

                var vocabIdsToAssign = vocabIdsForCatalogue.Except(existingFlashcardVocabIds).ToList();
                if (vocabIdsToAssign.Any())
                {
                    var today = PolandTime.Today;
                    var flashcards = vocabIdsToAssign.Select(vId => new Flashcard
                    {
                        UserId = wordfinder.StudentUserId,
                        VocabularyId = vId,
                        EaseFactor = 250,
                        Interval = 0,
                        IsLeech = false,
                        NextReviewDate = today
                    }).ToList();

                    _dbContext.Flashcards.AddRange(flashcards);
                }
            }

            // 5. Update Wordfinder status
            wordfinder.Status = WordfinderCatalogueStatus.Approved;
            wordfinder.ApprovedCatalogueId = systemCatalogue.Id;
            wordfinder.ReviewedAt = PolandTime.DateTimeNow;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(wordfinder.Id, adminUserId, isAdmin: true);
        }

        public async Task<WordfinderCatalogueDto> RejectCatalogueAsync(int id, int adminUserId, RejectWordfinderCatalogueRequest request)
        {
            var wordfinder = await _dbContext.WordfinderCatalogues
                .Include(w => w.StudentUser)
                    .ThenInclude(u => u.Profile)
                .Include(w => w.Entries)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (wordfinder is null)
                throw new NotFoundException($"Wordfinder catalogue with Id {id} was not found");

            if (wordfinder.Status != WordfinderCatalogueStatus.PendingApproval)
                throw new BadRequestException("Only catalogues in PendingApproval status can be rejected");

            wordfinder.Status = WordfinderCatalogueStatus.Rejected;
            wordfinder.RejectionReason = request.RejectionReason;
            wordfinder.ReviewedAt = PolandTime.DateTimeNow;

            await _dbContext.SaveChangesAsync();

            return await GetByIdAsync(wordfinder.Id, adminUserId, isAdmin: true);
        }

        private string ComputeInitials(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return "XX";

            var parts = username.Trim().Split(new[] { ' ', '.', '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                var first = parts[0][0];
                var last = parts[parts.Length - 1][0];
                return $"{char.ToUpper(first)}{char.ToUpper(last)}";
            }

            var name = parts[0];
            if (name.Length >= 2)
                return name.Substring(0, 2).ToUpper();

            return name.ToUpper();
        }

        private WordfinderCatalogueDto MapToDto(WordfinderCatalogue w)
        {
            var username = w.StudentUser?.Username ?? string.Empty;
            return new WordfinderCatalogueDto
            {
                Id = w.Id,
                StudentUserId = w.StudentUserId,
                StudentUsername = username,
                StudentInitials = ComputeInitials(username),
                StudentAvatarUrl = w.StudentUser?.Profile?.AvatarUrl,
                Name = w.Name,
                Status = w.Status,
                RejectionReason = w.RejectionReason,
                ApprovedCatalogueId = w.ApprovedCatalogueId,
                CreatedAt = w.CreatedAt,
                SubmittedAt = w.SubmittedAt,
                ReviewedAt = w.ReviewedAt,
                Entries = w.Entries.Select(e => new WordfinderCatalogueEntryDto
                {
                    Id = e.Id,
                    Front = e.Front,
                    Back = e.Back
                }).ToList()
            };
        }

        private WordfinderCatalogueListDto MapToListDto(WordfinderCatalogue w)
        {
            var username = w.StudentUser?.Username ?? string.Empty;
            return new WordfinderCatalogueListDto
            {
                Id = w.Id,
                StudentUserId = w.StudentUserId,
                StudentUsername = username,
                StudentInitials = ComputeInitials(username),
                StudentAvatarUrl = w.StudentUser?.Profile?.AvatarUrl,
                Name = w.Name,
                Status = w.Status,
                EntryCount = w.Entries?.Count ?? 0,
                CreatedAt = w.CreatedAt,
                SubmittedAt = w.SubmittedAt,
                ReviewedAt = w.ReviewedAt
            };
        }
    }
}
