using inzBackend.Exceptions;
using inzBackend.Models.StreamEntryModels;
using inzBackend.Models;
using Microsoft.EntityFrameworkCore;
using inzBackend.Helpers;

namespace inzBackend.Services.StreamEntryServices
{
    public class StreamService : IStreamService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public StreamService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<StreamEntryListDto> getAll(int? userId)
        {
            var query = _dbContext.StreamEntries
                .Include(x => x.User)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            return query
                .OrderByDescending(x => x.ExecutedAt)
                .Select(x => new StreamEntryListDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Username = x.User.Username,
                    Command = x.Command,
                    Payload = x.Payload,
                    ExecutedAt = x.ExecutedAt
                })
                .ToList();
        }

        public void create(CreateStreamEntryRequest request)
        {
            var userExists = _dbContext.Users.Any(x => x.Id == request.UserId);
            if (!userExists)
                throw new NotFoundException("User not found");

            _dbContext.StreamEntries.Add(new StreamEntry
            {
                UserId = request.UserId,
                Command = request.Command,
                Payload = request.Payload,
                ExecutedAt = PolandTime.Now
            });
            _dbContext.SaveChanges();
        }

        public void delete(int id)
        {
            var entry = _dbContext.StreamEntries.FirstOrDefault(x => x.Id == id);
            if (entry is null)
                throw new NotFoundException("Stream entry not found");

            _dbContext.StreamEntries.Remove(entry);
            _dbContext.SaveChanges();
        }

        public void deleteMultiple(List<int> ids)
        {
            var entries = _dbContext.StreamEntries
                .Where(x => ids.Contains(x.Id))
                .ToList();

            _dbContext.StreamEntries.RemoveRange(entries);
            _dbContext.SaveChanges();
        }
    }
}
