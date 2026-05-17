using inzBackend.Exceptions;
using inzBackend.Models.CatalogueModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace inzBackend.Services.CatalogueServices
{
    public class CatalogueService : ICatalogueService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public CatalogueService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public async Task<CatalogueDto> uploadCatalogue(IFormFile file)
        {
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                throw new BadRequestException("Only .xlsx and .xls files are allowed");

            var userId = _userContextService.GetUserId!.Value;
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var entries = new List<(DateOnly date, string user, string entry, string key, string catalogueName)>();

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

                    DateOnly entryDate;
                    if (!DateOnly.TryParse(dateCell, out entryDate))
                        entryDate = today;

                    entries.Add((entryDate, userRef, entryVal, computedKey, catalogueName));
                }
            }

            if (!entries.Any())
                throw new BadRequestException("File contains no valid entries");

            var catalogueNames = entries
                .Select(e => e.catalogueName)
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
                    _dbContext.SaveChanges();
                }

                var catEntries = entries
                    .Where(e => e.catalogueName == catName ||
                               (string.IsNullOrWhiteSpace(e.catalogueName) && catName == fallbackName))
                    .Select(e => new CatalogueEntry
                    {
                        CatalogueId = existing.Id,
                        EntryDate = e.date,
                        UserRef = e.user,
                        Entry = e.entry,
                        ComputedKey = e.key
                    })
                    .ToList();

                _dbContext.CatalogueEntries.AddRange(catEntries);
                _dbContext.SaveChanges();
                lastCatalogue = existing;
            }

            return MapCatalogue(lastCatalogue!);
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
                    ComputedKey = x.ComputedKey,
                    CatalogueName = x.Catalogue.Name
                })
                .ToList();
        }

        public void deleteCatalogue(int catalogueId)
        {
            var catalogue = _dbContext.Catalogues
                .Include(x => x.Entries)
                .FirstOrDefault(x => x.Id == catalogueId);

            if (catalogue is null)
                throw new NotFoundException("Catalogue not found");

            _dbContext.CatalogueEntries.RemoveRange(catalogue.Entries);
            _dbContext.Catalogues.Remove(catalogue);
            _dbContext.SaveChanges();
        }

        private static CatalogueDto MapCatalogue(Catalogue x) => new()
        {
            Id = x.Id,
            Name = x.Name,
            UploadedDate = x.UploadedDate,
            UploadedBy = x.UploadedBy?.Username ?? string.Empty,
            EntryCount = x.Entries?.Count() ?? 0
        };
    }
}
