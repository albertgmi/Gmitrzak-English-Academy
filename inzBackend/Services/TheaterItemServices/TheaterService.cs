using inzBackend.Exceptions;
using inzBackend.Models.TheaterItemsModels;
using inzBackend.Models;
using inzBackend.Enums;
using inzBackend.Entities.Resources;

namespace inzBackend.Services.TheaterItemServices
{
    public class TheaterService : ITheaterService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public TheaterService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<TheaterItemDto> GetAll()
        {
            return _dbContext.TheaterItems
                .OrderBy(x => x.Title)
                .Select(x => MapToDto(x))
                .ToList();
        }

        public List<RepertoireItemDto> GetRepertoire()
        {
            var items = _dbContext.TheaterItems
                .Where(x => x.IsActive)
                .ToList();

            var reportCounts = _dbContext.ListeningReports
                .GroupBy(x => x.Title)
                .Select(g => new
                {
                    Title = g.Key,
                    TimesReported = g.Count(),
                    LastReported = g.Max(x => x.ReportDate)
                })
                .ToList();

            return items.Select(item =>
            {
                var report = reportCounts
                    .FirstOrDefault(r => r.Title.ToLower() == item.Title.ToLower());

                return new RepertoireItemDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Url = item.Url,
                    ThumbnailUrl = item.ThumbnailUrl,
                    MediaType = item.MediaType.ToString(),
                    DurationMinutes = item.DurationMinutes,
                    Level = item.Level,
                    TimesReported = report?.TimesReported ?? 0,
                    LastReportedDate = report?.LastReported
                };
            })
            .OrderByDescending(x => x.TimesReported)
            .ToList();
        }

        public TheaterItemDto GetById(int id)
        {
            var item = _dbContext.TheaterItems.FirstOrDefault(x => x.Id == id);
            if (item is null)
                throw new NotFoundException("Theater item not found");
            return MapToDto(item);
        }

        public TheaterItemDto Create(CreateTheaterItemRequest request)
        {
            if (!Enum.TryParse<MediaType>(request.MediaType, out var mediaType))
                throw new BadRequestException($"Invalid media type: {request.MediaType}");

            var item = new TheaterItem
            {
                Title = request.Title,
                Description = request.Description,
                Url = request.Url,
                ThumbnailUrl = request.ThumbnailUrl,
                MediaType = mediaType,
                DurationMinutes = request.DurationMinutes,
                Level = request.Level,
                IsActive = true
            };

            _dbContext.TheaterItems.Add(item);
            _dbContext.SaveChanges();
            return MapToDto(item);
        }

        public void Update(int id, UpdateTheaterItemRequest request)
        {
            var item = _dbContext.TheaterItems.FirstOrDefault(x => x.Id == id);
            if (item is null)
                throw new NotFoundException("Theater item not found");

            if (!Enum.TryParse<MediaType>(request.MediaType, out var mediaType))
                throw new BadRequestException($"Invalid media type: {request.MediaType}");

            item.Title = request.Title;
            item.Description = request.Description;
            item.Url = request.Url;
            item.ThumbnailUrl = request.ThumbnailUrl;
            item.MediaType = mediaType;
            item.DurationMinutes = request.DurationMinutes;
            item.Level = request.Level;
            item.IsActive = request.IsActive;

            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var item = _dbContext.TheaterItems.FirstOrDefault(x => x.Id == id);
            if (item is null)
                throw new NotFoundException("Theater item not found");

            _dbContext.TheaterItems.Remove(item);
            _dbContext.SaveChanges();
        }

        public void ToggleActive(int id)
        {
            var item = _dbContext.TheaterItems.FirstOrDefault(x => x.Id == id);
            if (item is null)
                throw new NotFoundException("Theater item not found");

            item.IsActive = !item.IsActive;
            _dbContext.SaveChanges();
        }

        private static TheaterItemDto MapToDto(TheaterItem x) => new()
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Url = x.Url,
            ThumbnailUrl = x.ThumbnailUrl,
            MediaType = x.MediaType.ToString(),
            DurationMinutes = x.DurationMinutes,
            Level = x.Level,
            IsActive = x.IsActive
        };
    }
}
