using ClosedXML.Excel;
using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models.SentenceSetsModels;
using inzBackend.Models.SentenceStockModels;
using inzBackend.Models;
using inzBackend.Services.AiIntegrationServices;
using inzBackend.Models.ModuleSentenceModels;
using inzBackend.Helpers;
using inzBackend.Models.AdminLearningModels;
using Microsoft.EntityFrameworkCore;
using inzBackend.Entities.LearningMaterials;
using inzBackend.Entities.Assignments;

namespace inzBackend.Services.SentenceServices
{
    public class SentenceService : ISentenceService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IAiTranslationService _aiTranslationService;

        public SentenceService(GmitrzakEnglishAcademyDbContext dbContext, IAiTranslationService aiTranslationService)
        {
            _dbContext = dbContext;
            _aiTranslationService = aiTranslationService;
        }
        public List<SentenceStockDto> getAllStock()
        {
            return _dbContext.SentenceStocks
                .OrderBy(x => x.Category).ThenBy(x => x.Polish)
                .Select(x => new SentenceStockDto
                {
                    Id = x.Id,
                    Polish = x.Polish,
                    EnglishTranslation = x.EnglishTranslation,
                    Category = x.Category
                })
                .ToList();
        }

        public void createStock(CreateSentenceStockRequest request)
        {
            _dbContext.SentenceStocks.Add(new SentenceStock
            {
                Polish = request.Polish,
                EnglishTranslation = request.EnglishTranslation,
                Category = request.Category
            });
            _dbContext.SaveChanges();
        }

        public void deleteStock(int id)
        {
            var s = _dbContext.SentenceStocks.FirstOrDefault(x => x.Id == id)
                ?? throw new NotFoundException("Sentence not found");

            _dbContext.SentenceStocks.Remove(s);
            _dbContext.SaveChanges();
        }

        public async Task<int> uploadStockFromExcel(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheets.First();
            var rows = ws.RangeUsed().RowsUsed().Skip(1).ToList();

            var toTranslate = new List<SentenceImportEntry>();

            var existingStockSentences = _dbContext.SentenceStocks
                .Select(x => x.EnglishTranslation.Trim().ToLower())
                .ToHashSet();

            foreach (var row in rows)
            {
                var english = row.Cell(3).GetValue<string>().Trim();
                var category = row.Cell(5).GetValue<string>().Trim();

                if (string.IsNullOrWhiteSpace(english))
                    continue;

                if (existingStockSentences.Contains(english.ToLower()))
                    continue;

                toTranslate.Add(new SentenceImportEntry
                {
                    English = english,
                    Category = category
                });

                existingStockSentences.Add(english.ToLower());
            }

            if (!toTranslate.Any()) return 0;

            var englishTexts = toTranslate.Select(x => x.English).ToList();
            var polishTranslations = await _aiTranslationService.TranslateBatchAsync(englishTexts, "Polish");

            var toAdd = new List<SentenceStock>();
            for (var i = 0; i < toTranslate.Count; i++)
            {
                var polish = i < polishTranslations.Count ? polishTranslations[i] : string.Empty;
                var entry = toTranslate[i];

                toAdd.Add(new SentenceStock
                {
                    Polish = polish,
                    EnglishTranslation = entry.English,
                    Category = entry.Category
                });
            }

            _dbContext.SentenceStocks.AddRange(toAdd);
            _dbContext.SaveChanges();
            return toAdd.Count;
        }

        public List<SentenceSetGroupDto> getAllSetsGrouped()
        {
            var sets = _dbContext.SentenceSets
                .Include(x => x.Items).ThenInclude(i => i.SentenceStock)
                .OrderBy(x => x.GroupName).ThenBy(x => x.Order)
                .ToList();

            return sets
                .GroupBy(x => x.GroupName)
                .Select(g => new SentenceSetGroupDto
                {
                    GroupName = g.Key,
                    Sets = g.Select(s => new SentenceSetDto
                    {
                        Id = s.Id,
                        Name = s.Name,
                        GroupName = s.GroupName,
                        Order = s.Order,
                        ItemCount = s.Items.Count,
                        Items = s.Items.OrderBy(i => i.Order).Select(i => new SentenceSetItemDto
                        {
                            Id = i.Id,
                            SentenceStockId = i.SentenceStockId,
                            Polish = i.SentenceStock.Polish,
                            EnglishTranslation = i.SentenceStock.EnglishTranslation,
                            Order = i.Order
                        }).ToList()
                    }).ToList()
                })
                .ToList();
        }

        public SentenceSetDto getSet(int id)
        {
            var set = _dbContext.SentenceSets
                .Include(x => x.Items).ThenInclude(i => i.SentenceStock)
                .FirstOrDefault(x => x.Id == id)
                ?? throw new NotFoundException("Set not found");

            return new SentenceSetDto
            {
                Id = set.Id,
                Name = set.Name,
                GroupName = set.GroupName,
                Order = set.Order,
                ItemCount = set.Items.Count,
                Items = set.Items.OrderBy(i => i.Order).Select(i => new SentenceSetItemDto
                {
                    Id = i.Id,
                    SentenceStockId = i.SentenceStockId,
                    Polish = i.SentenceStock.Polish,
                    EnglishTranslation = i.SentenceStock.EnglishTranslation,
                    Order = i.Order
                }).ToList()
            };
        }

        public SentenceSetDto createSet(CreateSentenceSetRequest request)
        {
            var set = new SentenceSet
            {
                Name = request.Name,
                GroupName = request.GroupName,
                Order = request.Order
            };
            _dbContext.SentenceSets.Add(set);
            _dbContext.SaveChanges();

            var items = request.SentenceStockIds
                .Select((sid, idx) => new SentenceSetItem
                {
                    SentenceSetId = set.Id,
                    SentenceStockId = sid,
                    Order = idx + 1
                }).ToList();

            _dbContext.SentenceSetItems.AddRange(items);
            _dbContext.SaveChanges();

            return getSet(set.Id);
        }

        public void deleteSet(int id)
        {
            var set = _dbContext.SentenceSets
                .Include(x => x.Items)
                .FirstOrDefault(x => x.Id == id)
                ?? throw new NotFoundException("Set not found");

            _dbContext.SentenceSetItems.RemoveRange(set.Items);
            _dbContext.SentenceSets.Remove(set);
            _dbContext.SaveChanges();
        }

        public void assignToUser(AssignSentenceRequest request)
        {
            var stockSentence = _dbContext.SentenceStocks
                .FirstOrDefault(x => x.Id == request.SentenceStockId)
                ?? throw new NotFoundException("Sentence stock not found");

            var alreadyExists = _dbContext.Sentences
                .Any(x => x.UserId == request.UserId
                       && x.Translation.Trim().ToLower() == stockSentence.EnglishTranslation.Trim().ToLower());

            if (!alreadyExists)
            {
                var today = PolandTime.Today;

                _dbContext.Sentences.Add(new Sentence
                {
                    UserId = request.UserId,
                    Content = stockSentence.Polish,
                    Translation = stockSentence.EnglishTranslation,
                    Notes = "Assigned from sentence stock",
                    IsReviewed = false,
                    EaseFactor = 250,
                    Interval = 0,
                    IsLeech = false,
                    NextReviewDate = today
                });
            }

            _dbContext.UserSentenceAssignments.Add(new UserSentenceAssignment
            {
                UserId = request.UserId,
                SentenceSetId = request.SentenceSetId,
                SentenceStockId = request.SentenceStockId,
                DueDate = DateOnly.Parse(request.DueDate),
                IsCompleted = false
            });
            _dbContext.SaveChanges();
        }

        public void assignToModule(AssignSetToModuleRequest request)
        {
            var exists = _dbContext.ModuleSentenceSets
                .Any(x => x.ModuleId == request.ModuleId && x.SentenceSetId == request.SentenceSetId);

            if (exists)
                throw new BadRequestException("This sentence set is already assigned to this module.");

            _dbContext.ModuleSentenceSets.Add(new ModuleSentenceSet
            {
                ModuleId = request.ModuleId,
                SentenceSetId = request.SentenceSetId
            });
            _dbContext.SaveChanges();
        }

        public List<SentenceSetDto> getSetsForModule(int moduleId)
        {
            var setIds = _dbContext.ModuleSentenceSets
                .Where(x => x.ModuleId == moduleId)
                .Select(x => x.SentenceSetId)
                .ToList();

            return _dbContext.SentenceSets
                .Include(x => x.Items).ThenInclude(i => i.SentenceStock)
                .Where(x => setIds.Contains(x.Id))
                .Select(x => new SentenceSetDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    GroupName = x.GroupName,
                    Order = x.Order,
                    ItemCount = x.Items.Count,
                    Items = x.Items.OrderBy(i => i.Order).Select(i => new SentenceSetItemDto
                    {
                        Id = i.Id,
                        SentenceStockId = i.SentenceStockId,
                        Polish = i.SentenceStock.Polish,
                        EnglishTranslation = i.SentenceStock.EnglishTranslation,
                        Order = i.Order
                    }).ToList()
                })
                .ToList();
        }

        public void removeSetFromModule(int moduleId, int setId)
        {
            var link = _dbContext.ModuleSentenceSets
                .FirstOrDefault(x => x.ModuleId == moduleId && x.SentenceSetId == setId)
                ?? throw new NotFoundException("Assignment between module and set was not found");

            _dbContext.ModuleSentenceSets.Remove(link);
            _dbContext.SaveChanges();
        }

        public void updateStock(int id, UpdateSentenceStockRequest request)
        {
            var stock = _dbContext.SentenceStocks.FirstOrDefault(x => x.Id == id)
                ?? throw new NotFoundException("Sentence not found");

            var cleanPolish = RemovePunctuation(stock.Polish.ToLower().Trim());
            var cleanEnglish = RemovePunctuation(stock.EnglishTranslation.ToLower().Trim());

            var stockInSentences = _dbContext.Sentences
                .Where(x => x.Content.ToLower().Trim()
                    .Replace(".", "").Replace("?", "").Replace("!", "").Replace(",", "") == cleanPolish
                    && x.Translation.ToLower().Trim()
                    .Replace(".", "").Replace("?", "").Replace("!", "").Replace(",", "") == cleanEnglish)
                .FirstOrDefault();

            if (stockInSentences is not null)
                stockInSentences.Content = request.Polish;

            stock.Polish = request.Polish;
            _dbContext.SaveChanges();
        }

        public List<SearchSentenceResultDto> searchSentence(string query, int studentId)
        {
            var normalizedQuery = query.Trim().ToLower();

            var globalSentences = _dbContext.SentenceStocks
                .Where(s => s.EnglishTranslation.ToLower().Contains(normalizedQuery) ||
                            s.Polish.ToLower().Contains(normalizedQuery))
                .ToList();

            var studentSentences = _dbContext.Sentences
                .Where(s => s.UserId == studentId)
                .Select(s => new { Content = s.Content.ToLower(), Translation = s.Translation.ToLower() })
                .ToList();

            var results = new List<SearchSentenceResultDto>();

            if (globalSentences.Any())
            {
                foreach (var globalSentence in globalSentences)
                {
                    var globalEnglish = globalSentence.EnglishTranslation.ToLower();
                    var globalPolish = globalSentence.Polish.ToLower();

                    bool isAssigned = studentSentences.Any(s => s.Content == globalPolish || s.Translation == globalEnglish);

                    results.Add(new SearchSentenceResultDto
                    {
                        Id = globalSentence.Id,
                        EnglishTranslation = globalSentence.EnglishTranslation,
                        Polish = globalSentence.Polish,
                        Category = globalSentence.Category,
                        ExistsInGlobal = true,
                        AlreadyAssignedToStudent = isAssigned
                    });
                }
            }
            else
            {
                bool isAssigned = studentSentences.Any(s => s.Content.Contains(normalizedQuery) || s.Translation.Contains(normalizedQuery));

                results.Add(new SearchSentenceResultDto
                {
                    Id = null,
                    EnglishTranslation = query,
                    Polish = string.Empty,
                    Category = "Vocabulary",
                    ExistsInGlobal = false,
                    AlreadyAssignedToStudent = isAssigned
                });
            }

            return results;
        }

        string RemovePunctuation(string input)
        {
            return new string(input.Where(c => !char.IsPunctuation(c)).ToArray());
        }
    }
}