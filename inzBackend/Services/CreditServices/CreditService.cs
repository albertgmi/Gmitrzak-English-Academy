using inzBackend.Entities;
using inzBackend.Entities.Gamification;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.CreditModels;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.CreditServices
{
    public class CreditService : ICreditService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private const int DAILY_FLASHCARD_GOAL = 75;
        private const int WEEKLY_FLASHCARD_GOAL = 300;
        private const int DAILY_CREDIT_REWARD = 1;
        private const int WEEKLY_CREDIT_REWARD = 5;

        public CreditService(GmitrzakEnglishAcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void awardCredits(int userId, int amount, string reason)
        {
            _dbContext.Credits.Add(new Credit
            {
                UserId = userId,
                Amount = amount,
                Reason = reason,
                Date = PolandTime.Today
            });
            _dbContext.SaveChanges();
        }

        public void checkAndAwardDailyChallenge(int userId)
        {
            var today = PolandTime.Today;

            var alreadyAwarded = _dbContext.Credits
                .Any(x => x.UserId == userId
                       && x.Date == today
                       && x.Reason == "Daily challenge: 75 flashcards");

            if (alreadyAwarded) return;

            var todayCount = _dbContext.FlashcardStudyLogs
                .Where(x => x.UserId == userId
                         && x.StudyDate == today)
                .Sum(x => (int?)(x.EasyCount + x.HardCount + x.IncorrectCount)) ?? 0;

            if (todayCount >= DAILY_FLASHCARD_GOAL)
            {
                awardCredits(userId, DAILY_CREDIT_REWARD,
                    "Daily challenge: 75 flashcards");
            }
        }

        public void checkAndAwardWeeklyChallenge(int userId)
        {
            var today = PolandTime.Today;
            var dow = ((int)today.DayOfWeek + 6) % 7;
            var weekStart = today.AddDays(-dow);
            var weekEnd = weekStart.AddDays(6);

            var alreadyAwarded = _dbContext.Credits
                .Any(x => x.UserId == userId
                       && x.Date >= weekStart
                       && x.Date <= weekEnd
                       && x.Reason == "Weekly challenge: 300 flashcards");

            if (alreadyAwarded) return;

            var weekCount = _dbContext.FlashcardStudyLogs
                .Where(x => x.UserId == userId
                         && x.StudyDate >= weekStart
                         && x.StudyDate <= weekEnd)
                .Sum(x => (int?)(x.EasyCount + x.HardCount + x.IncorrectCount)) ?? 0;

            if (weekCount >= WEEKLY_FLASHCARD_GOAL)
            {
                awardCredits(userId, WEEKLY_CREDIT_REWARD,
                    "Weekly challenge: 300 flashcards");
            }
        }

        public CreditSummaryDto getCreditSummary(int userId)
        {
            var credits = _dbContext.Credits
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Date)
                .ToList();

            var purchases = _dbContext.ShopPurchases
                .Include(x => x.ShopItem)
                .Where(x => x.UserId == userId && x.Status != "Cancelled")
                .OrderByDescending(x => x.PurchaseDate)
                .ToList();

            var earned = credits.Sum(x => x.Amount);
            var spent = purchases.Sum(x => x.CreditCost);

            var history = credits.Select(x => new CreditHistoryItemDto
            {
                Id = x.Id,
                Amount = x.Amount,
                Reason = x.Reason,
                Date = x.Date,
                Type = "earned"
            }).ToList();

            var spendHistory = purchases.Select(x => new CreditHistoryItemDto
            {
                Id = x.Id,
                Amount = -x.CreditCost,
                Reason = $"Purchased: {x.ShopItem.Name}",
                Date = x.PurchaseDate,
                Type = "spent"
            }).ToList();

            var fullHistory = history
                .Concat(spendHistory)
                .OrderByDescending(x => x.Date)
                .ToList();

            return new CreditSummaryDto
            {
                TotalCredits = earned - spent,
                CreditsEarned = earned,
                CreditsSpent = spent,
                History = fullHistory,
                Purchases = purchases.Select(x => new ShopPurchaseDto
                {
                    Id = x.Id,
                    ItemName = x.ShopItem.Name,
                    IconEmoji = x.ShopItem.IconEmoji,
                    CreditCost = x.CreditCost,
                    PurchaseDate = x.PurchaseDate,
                    Status = x.Status
                }).ToList()
            };
        }

        public List<ShopItemDto> getShopItems(int userId)
        {
            var summary = getCreditSummary(userId);
            var available = summary.TotalCredits;

            return _dbContext.ShopItems
                .Where(x => x.IsActive)
                .OrderBy(x => x.CreditCost)
                .Select(x => new ShopItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CreditCost = x.CreditCost,
                    IconEmoji = x.IconEmoji,
                    CanAfford = available >= x.CreditCost
                })
                .ToList();
        }

        public List<ShopItemDto> getShopItems()
        {
            return _dbContext.ShopItems
                .Where(x => x.IsActive)
                .OrderBy(x => x.CreditCost)
                .Select(x => new ShopItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CreditCost = x.CreditCost,
                    IconEmoji = x.IconEmoji,
                    CanAfford = false
                })
                .ToList();
        }

        public ShopPurchaseResultDto purchaseItem(int userId, int shopItemId)
        {
            var item = _dbContext.ShopItems
                .FirstOrDefault(x => x.Id == shopItemId && x.IsActive);

            if (item is null)
                return new ShopPurchaseResultDto
                { Success = false, Message = "Item not found." };

            var summary = getCreditSummary(userId);
            var available = summary.TotalCredits;

            if (available < item.CreditCost)
                return new ShopPurchaseResultDto
                {
                    Success = false,
                    Message = $"Not enough credits. You have {available}, need {item.CreditCost}.",
                    CreditsRemaining = available
                };

            var purchase = new ShopPurchase
            {
                UserId = userId,
                ShopItemId = shopItemId,
                CreditCost = item.CreditCost,
                PurchaseDate = PolandTime.Today,
                Status = "Pending"
            };

            _dbContext.ShopPurchases.Add(purchase);
            _dbContext.SaveChanges();

            return new ShopPurchaseResultDto
            {
                Success = true,
                Message = $"Successfully purchased {item.Name}!",
                CreditsRemaining = available - item.CreditCost,
                PurchaseId = purchase.Id
            };
        }

        public List<UserCreditSummaryDto> getAllUsersCreditSummary()
        {
            var users = _dbContext.Users
                .Include(x=>x.Profile)
                .Where(x => x.Role == Enums.UserRole.User && x.IsActive)
                .ToList();

            var result = users.Select(u =>
            {
                var summary = getCreditSummary(u.Id);
                return new UserCreditSummaryDto
                {
                    UserId = u.Id,
                    Username = u.Username,
                    TotalCredits = summary.TotalCredits,
                    CreditsEarned = summary.CreditsEarned,
                    CreditsSpent = summary.CreditsSpent,
                    PurchaseCount = summary.Purchases.Count,
                    AvatarUrl = u.Profile.AvatarUrl
                };
            }).ToList();

            return result;
        }

        public StudentCreditDetailDto getStudentCreditDetail(int studentId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == studentId)
                ?? throw new NotFoundException("Student not found");

            var summary = getCreditSummary(studentId);

            return new StudentCreditDetailDto
            {
                UserId = user.Id,
                Username = user.Username,
                TotalCredits = summary.TotalCredits,
                CreditsEarned = summary.CreditsEarned,
                CreditsSpent = summary.CreditsSpent,
                History = summary.History,
                Purchases = summary.Purchases
            };
        }

        public void updatePurchaseStatus(int purchaseId, string status)
        {
            var purchase = _dbContext.ShopPurchases
                .FirstOrDefault(x => x.Id == purchaseId)
                ?? throw new NotFoundException("Purchase not found");

            var validStatuses = new[] { "Pending", "Fulfilled", "Cancelled" };
            if (!validStatuses.Contains(status))
                throw new BadRequestException("Invalid status");

            purchase.Status = status;
            _dbContext.SaveChanges();
        }
    }
}