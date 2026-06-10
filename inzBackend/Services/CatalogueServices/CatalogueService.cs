using inzBackend.Exceptions;
using inzBackend.Models.CatalogueModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using inzBackend.Services.AiIntegrationServices;
using AutoMapper;
using inzBackend.Entities;
using inzBackend.Helpers;

namespace inzBackend.Services.CatalogueServices
{
    public class CatalogueService : ICatalogueService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IAiTranslationService _aiTranslationService;
        private readonly IMapper _mapper;

        public CatalogueService(
            GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService,
            IAiTranslationService aiTranslationService, IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _aiTranslationService = aiTranslationService;
            _mapper = mapper;
        }

        public async Task<CatalogueDto> uploadCatalogue(IFormFile file)
        {
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                throw new BadRequestException("Only .xlsx and .xls files are allowed");

            var userId = _userContextService.GetUserId!.Value;
            var today = PolandTime.Today;

            var entries = new List<ParsedCatalogueRowDto>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.First();
                var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    var dateCell = row.Cell(1).GetValue<string>();
                    var userRef = row.Cell(2).GetValue<string>().Trim();
                    var entryVal = row.Cell(3).GetValue<string>().Trim();
                    var computedKey = row.Cell(4).GetValue<string>().Trim();
                    var catalogueName = row.Cell(5).GetValue<string>().Trim();

                    if (string.IsNullOrWhiteSpace(entryVal)) continue;

                    if (!DateOnly.TryParse(dateCell, out DateOnly entryDate))
                        entryDate = today;

                    entries.Add(new ParsedCatalogueRowDto(
                        entryDate,
                        userRef,
                        entryVal,
                        computedKey,
                        catalogueName));
                }
            }

            if (!entries.Any())
                throw new BadRequestException("File contains no valid entries");

            var catalogueNames = entries
                .Select(e => e.CatalogueName)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct()
                .ToList();

            var fallbackName = Path.GetFileNameWithoutExtension(file.FileName);
            if (!catalogueNames.Any())
                catalogueNames.Add(fallbackName);

            Catalogue? lastCatalogue = null;

            foreach (var catName in catalogueNames)
            {
                var existing = _dbContext.Catalogues
                    .FirstOrDefault(x => x.Name == catName);

                if (existing is null)
                {
                    existing = new Catalogue
                    {
                        Name = catName,
                        UploadedDate = today,
                        UploadedByUserId = userId
                    };
                    _dbContext.Catalogues.Add(existing);

                    await _dbContext.SaveChangesAsync();
                }

                var catEntries = entries
                    .Where(e => e.CatalogueName == catName ||
                               (string.IsNullOrWhiteSpace(e.CatalogueName) && catName == fallbackName))
                    .Select(e => new CatalogueEntry
                    {
                        CatalogueId = existing.Id,
                        EntryDate = e.EntryDate,
                        UserRef = e.UserRef,
                        Entry = e.EntryVal,
                        ComputedKey = e.ComputedKey
                    })
                    .ToList();

                var textsToTranslate = catEntries
                    .Select(e => e.Entry)
                    .ToList();

                var translatedTexts = await _aiTranslationService
                    .TranslateBatchAsync(textsToTranslate, "Polish");

                var newVocabularies = new List<Vocabulary>();

                for (int i = 0; i < catEntries.Count; i++)
                {
                    if (i < translatedTexts.Count)
                    {
                        var originalText = catEntries[i].Entry;
                        var translatedText = translatedTexts[i];

                        catEntries[i].TranslatedEntry = translatedText;

                        if (!string.IsNullOrWhiteSpace(originalText) && !string.IsNullOrWhiteSpace(translatedText))
                        {
                            newVocabularies.Add(new Vocabulary
                            {
                                Front = originalText,
                                Back = translatedText,
                                Category = catName,
                                CatalogueId = existing.Id
                            });
                        }
                    }
                }

                var distinctVocabularies = newVocabularies
                    .GroupBy(v => v.Front)
                    .Select(g => g.First())
                    .ToList();

                var rawWordsToCompare = distinctVocabularies
                    .Select(d => d.Front.ToLower())
                    .ToList();

                var existingWordsInDb = await _dbContext.Vocabulary
                    .Where(v => rawWordsToCompare.Contains(v.Front.ToLower()))
                    .Select(v => v.Front)
                    .ToListAsync();

                var vocabulariesToInsert = distinctVocabularies
                    .Where(v => !existingWordsInDb.Any(e => e.Equals(v.Front, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (vocabulariesToInsert.Any())
                    _dbContext.Vocabulary.AddRange(vocabulariesToInsert);

                _dbContext.CatalogueEntries.AddRange(catEntries);

                await _dbContext.SaveChangesAsync();

                lastCatalogue = existing;
            }

            return _mapper.Map<CatalogueDto>(lastCatalogue);
        }

        public List<CatalogueDto> getAllCatalogues()
        {
            return _dbContext.Catalogues
                .Include(x => x.UploadedBy)
                .Include(x => x.Entries)
                .OrderByDescending(x => x.UploadedDate)
                .Select(x => new CatalogueDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UploadedDate = x.UploadedDate,
                    UploadedBy = x.UploadedBy.Username,
                    EntryCount = x.Entries.Count()
                })
                .ToList();
        }

        public List<CatalogueEntryDto> getEntries(CatalogueEntryFilterRequest filter)
        {
            var query = _dbContext.CatalogueEntries
                .Include(x => x.Catalogue)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.CatalogueName))
                query = query.Where(x => x.Catalogue.Name == filter.CatalogueName);

            if (!string.IsNullOrWhiteSpace(filter.UserRef))
                query = query.Where(x => x.UserRef.ToLower().Contains(filter.UserRef.ToLower()));

            if (filter.DateFrom.HasValue)
                query = query.Where(x => x.EntryDate >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                query = query.Where(x => x.EntryDate <= filter.DateTo.Value);

            return query
                .OrderByDescending(x => x.EntryDate)
                .Select(x => new CatalogueEntryDto
                {
                    Id = x.Id,
                    EntryDate = x.EntryDate,
                    UserRef = x.UserRef,
                    Entry = x.Entry,
                    TranslatedEntry = x.TranslatedEntry,
                    ComputedKey = x.ComputedKey,
                    CatalogueName = x.Catalogue.Name
                })
                .ToList();
        }

        public void deleteCatalogue(int catalogueId)
        {
            var catalogue = _dbContext
                .Catalogues
                .Include(x => x.Entries)
                .FirstOrDefault(x => x.Id == catalogueId);

            if (catalogue is null)
                throw new NotFoundException("Catalogue not found");

            var relatedVocabulary = _dbContext.Vocabulary
                .Where(x => x.CatalogueId == catalogueId)
                .ToList();

            _dbContext.CatalogueEntries.RemoveRange(catalogue.Entries);
            _dbContext.Vocabulary.RemoveRange(relatedVocabulary);
            _dbContext.Catalogues.Remove(catalogue);
            _dbContext.SaveChanges();
        }

        public void updateTranslation(UpdateTranslationRequest request, int entryId)
        {
            var entry = _dbContext.CatalogueEntries.FirstOrDefault(x => x.Id == entryId);
            if (entry is null)
                throw new NotFoundException($"Entry with Id: {entryId} was not found");

            var oldTranslation = entry.TranslatedEntry;
            var newTranslation = request.TranslatedEntry;

            if (oldTranslation == newTranslation)
                return;

            entry.TranslatedEntry = newTranslation;

            var vocabularyWord = _dbContext.Vocabulary
                .FirstOrDefault(x => x.Front == entry.Entry);

            if (vocabularyWord is not null)
                vocabularyWord.Back = newTranslation;

            _dbContext.SaveChanges();
        }
    }
}
