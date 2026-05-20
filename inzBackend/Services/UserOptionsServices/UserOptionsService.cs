using inzBackend.Models.UserOptionsModels;
using inzBackend.Models;
using inzBackend.Entities;
using inzBackend.Exceptions;

namespace inzBackend.Services.UserOptionsServices
{
    public class UserOptionsService : IUserOptionsService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;

        public UserOptionsService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<UserOptionsDto> getAllOptions()
        {
            var users = _dbContext.Users
                .Where(x => x.Role == Enums.UserRole.User && x.IsActive)
                .ToList();

            return users.Select(u =>
            {
                var opts = _dbContext.UserOptions.FirstOrDefault(x => x.UserId == u.Id)
                    ?? new UserOptions { UserId = u.Id };
                return MapToDto(opts, u.Username);
            }).ToList();
        }

        public UserOptionsDto getOptions(int userId)
        {
            var user = _dbContext
                .Users
                .FirstOrDefault(x => x.Id == userId);

            if (user is null)
                throw new NotFoundException("User not found");

            var opts = _dbContext
                .UserOptions
                .FirstOrDefault(x => x.UserId == userId)
                ?? new UserOptions { UserId = userId };

            return MapToDto(opts, user.Username);
        }

        public void updateOptions(int userId, UpdateUserOptionsRequest request)
        {
            var opts = _dbContext
                .UserOptions
                .FirstOrDefault(x => x.UserId == userId);
            if (opts is null)
            {
                opts = new UserOptions { UserId = userId };
                _dbContext.UserOptions.Add(opts);
            }

            opts.LeechThreshold = request.LeechThreshold;
            opts.MinDailyFlashcards = request.MinDailyFlashcards;
            opts.EmailNotifications = request.EmailNotifications;
            opts.IncorrectStepOneMinutes = request.IncorrectStepOneMinutes;
            opts.IncorrectStepTwoMinutes = request.IncorrectStepTwoMinutes;
            opts.IncorrectStepThreeMinutes = request.IncorrectStepThreeMinutes;
            opts.IncorrectStepFourMinutes = request.IncorrectStepFourMinutes;

            _dbContext.SaveChanges();
        }

        private static UserOptionsDto MapToDto(UserOptions o, string username) => new()
        {
            UserId = o.UserId,
            Username = username,
            LeechThreshold = o.LeechThreshold,
            MinDailyFlashcards = o.MinDailyFlashcards,
            EmailNotifications = o.EmailNotifications,
            IncorrectStepOneMinutes = o.IncorrectStepOneMinutes,
            IncorrectStepTwoMinutes = o.IncorrectStepTwoMinutes,
            IncorrectStepThreeMinutes = o.IncorrectStepThreeMinutes,
            IncorrectStepFourMinutes = o.IncorrectStepFourMinutes
        };
    }
}
