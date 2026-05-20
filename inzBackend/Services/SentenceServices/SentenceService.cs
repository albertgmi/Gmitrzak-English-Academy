using ClosedXML.Excel;
using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models.SentenceSetsModels;
using inzBackend.Models.SentenceStockModels;
using inzBackend.Models;
using inzBackend.Services.SentenceServices;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using inzBackend.Services.AiIntegrationServices;
using inzBackend.Models.ModuleSentenceModels;
using Microsoft.AspNetCore.Mvc;

public class SentenceService : ISentenceService
{
    private readonly GmitrzakEnglishAcademyDbContext _dbContext;
    private readonly IAiTranslationService _aiTranslationService;

    public SentenceService(GmitrzakEnglishAcademyDbContext dbContext,
        IAiTranslationService aiTranslationService)
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
        var s = _dbContext.SentenceStocks.FirstOrDefault(x => x.Id == id);
        if (s is null) 
            throw new NotFoundException("Sentence not found");
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

        var toTranslate = new List<(string entry, string category)>();

        foreach (var row in rows)
        {
            var entry = row.Cell(3).GetValue<string>().Trim();
            var category = row.Cell(5).GetValue<string>().Trim();

            if (string.IsNullOrWhiteSpace(entry)) continue;

            var exists = _dbContext.SentenceStocks.Any(x => x.Polish == entry);
            if (exists) continue;

            toTranslate.Add((entry, category));
        }

        if (!toTranslate.Any()) return 0;

        var englishSentences = toTranslate.Select(x => x.entry).ToList();
        var translations = await _aiTranslationService.TranslateBatchAsync(englishSentences, "Polish");

        var toAdd = new List<SentenceStock>();
        for (var i = 0; i < toTranslate.Count; i++)
        {
            var english = i < translations.Count ? translations[i] : string.Empty;
            toAdd.Add(new SentenceStock
            {
                Polish = toTranslate[i].entry,
                EnglishTranslation = english,
                Category = toTranslate[i].category
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
            .FirstOrDefault(x => x.Id == id);
        if (set is null) throw new NotFoundException("Set not found");

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
            .FirstOrDefault(x => x.Id == id);
        if (set is null) throw new NotFoundException("Set not found");

        _dbContext.SentenceSetItems.RemoveRange(set.Items);
        _dbContext.SentenceSets.Remove(set);
        _dbContext.SaveChanges();
    }

    public void assignToUser(AssignSentenceRequest request)
    {
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
            .Any(x => x.ModuleId == request.ModuleId
                   && x.SentenceSetId == request.SentenceSetId);
        if (exists)
            throw new NotFoundException("Module Sentence set was not found");

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
            .FirstOrDefault(x => x.ModuleId == moduleId && x.SentenceSetId == setId);
        if (link is null)
            throw new NotFoundException("Module Sentence set was not found");

        _dbContext.ModuleSentenceSets.Remove(link);
        _dbContext.SaveChanges();
    }
}