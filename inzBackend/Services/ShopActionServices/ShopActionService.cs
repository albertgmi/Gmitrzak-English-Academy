using inzBackend.Entities.Assignments;
using inzBackend.Entities.Gamification;
using inzBackend.Enums;
using inzBackend.Exceptions;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.CreditModels;
using inzBackend.Services.CreditServices;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.ShopActionServices
{
    public class ShopActionService : IShopActionService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly ICreditService _creditService;
        private readonly IUserContextService _userContextService;

        public ShopActionService(GmitrzakEnglishAcademyDbContext dbContext, ICreditService creditService,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _creditService = creditService;
            _userContextService = userContextService;
        }
        public ShopPurchaseResultDto SkipHomework(int assignmentId)
        {
            var userId = _userContextService.GetUserId!.Value;

            var shopItem = _dbContext.ShopItems.FirstOrDefault(x => x.Name == "Homework Skip" && x.IsActive);
            if (shopItem is null)
                return new ShopPurchaseResultDto { Success = false, Message = "Item 'Homework Skip' is currently unavailable." };

            var purchaseResult = _creditService.PurchaseItem(userId, shopItem.Id);
            if (!purchaseResult.Success || !purchaseResult.PurchaseId.HasValue)
                return purchaseResult;

            var actionResult = CompleteModuleForUser(userId, assignmentId);
            if (actionResult == ActionTargetResult.Success)
            {
                _creditService.UpdatePurchaseStatus(purchaseResult.PurchaseId.Value, "Fulfilled");
                purchaseResult.Message = "Homework skipped successfully!";
                return purchaseResult;
            }

            _creditService.UpdatePurchaseStatus(purchaseResult.PurchaseId.Value, "Cancelled");
            string errorMessage = actionResult == ActionTargetResult.AlreadyCompleted
                ? "This homework is already completed or skipped!"
                : "Could not find this homework assigned to you.";

            return new ShopPurchaseResultDto
            {
                Success = false,
                Message = errorMessage,
                CreditsRemaining = purchaseResult.CreditsRemaining + shopItem.CreditCost
            };
        }

        public ShopPurchaseResultDto ExtendHomework(int assignmentId, DateOnly newDueDate)
        {
            var userId = _userContextService.GetUserId!.Value;

            var shopItem = _dbContext.ShopItems.FirstOrDefault(x => x.Name == "Homework Extension" && x.IsActive);
            if (shopItem is null)
                return new ShopPurchaseResultDto { Success = false, Message = "Item 'Homework Extension' is currently unavailable." };

            var today = PolandTime.Today;

            if (assignmentId < 0)
            {
                int matrixModuleId = Math.Abs(assignmentId);

                var mm = _dbContext.MatrixModules
                    .Include(x => x.Matrix)
                    .FirstOrDefault(x => x.Id == matrixModuleId);
                if (mm is null)
                    return new ShopPurchaseResultDto { Success = false, Message = "Could not find this homework assigned to you." };

                var matrixAssignment = _dbContext.UserMatrixAssignments
                    .FirstOrDefault(x => x.UserId == userId && x.MatrixId == mm.MatrixId);
                if (matrixAssignment is null)
                    return new ShopPurchaseResultDto { Success = false, Message = "Could not find this homework assigned to you." };

                var alreadyDone = _dbContext.UserMatrixModuleCompletions
                    .Any(x => x.UserId == userId && x.MatrixModuleId == matrixModuleId);
                if (alreadyDone)
                    return new ShopPurchaseResultDto { Success = false, Message = "This homework is already completed!" };

                var originalDeadline = MatrixModuleDateHelper.ComputeDeadline(
                    matrixAssignment.StartDate, mm.WeekNumber, mm.DayOfWeek, matrixAssignment.Matrix.RefreshIntervalDays);

                var existingOverride = _dbContext.UserMatrixModuleDueDateOverrides
                    .FirstOrDefault(x => x.UserId == userId && x.MatrixModuleId == matrixModuleId);

                var currentDeadline = existingOverride?.NewDeadline ?? originalDeadline;

                var minDate = currentDeadline.AddDays(1) > today ? currentDeadline.AddDays(1) : today;
                var maxDate = currentDeadline.AddDays(7);

                if (maxDate < minDate)
                    return new ShopPurchaseResultDto { Success = false, Message = "This homework is too overdue to extend." };

                if (newDueDate < minDate || newDueDate > maxDate)
                    return new ShopPurchaseResultDto
                    {
                        Success = false,
                        Message = $"New due date must be between {minDate:dd.MM.yyyy} and {maxDate:dd.MM.yyyy}."
                    };

                var purchaseResult = _creditService.PurchaseItem(userId, shopItem.Id);
                if (!purchaseResult.Success || !purchaseResult.PurchaseId.HasValue)
                    return purchaseResult;

                if (existingOverride is not null)
                    existingOverride.NewDeadline = newDueDate;
                else
                    _dbContext.UserMatrixModuleDueDateOverrides.Add(new UserMatrixModuleDueDateOverride
                    {
                        UserId = userId,
                        MatrixModuleId = matrixModuleId,
                        NewDeadline = newDueDate
                    });

                _dbContext.SaveChanges();
                _creditService.UpdatePurchaseStatus(purchaseResult.PurchaseId.Value, "Fulfilled");
                purchaseResult.Message = $"Deadline extended to {newDueDate:dd.MM.yyyy}!";
                return purchaseResult;
            }

            var assignment = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.Id == assignmentId && x.UserId == userId);

            if (assignment is null)
                return new ShopPurchaseResultDto { Success = false, Message = "Could not find this homework assigned to you." };
            if (assignment.IsCompleted)
                return new ShopPurchaseResultDto { Success = false, Message = "This homework is already completed!" };

            var minD = assignment.DueDate.AddDays(1) > today ? assignment.DueDate.AddDays(1) : today;
            var maxD = assignment.DueDate.AddDays(7);

            if (maxD < minD)
                return new ShopPurchaseResultDto { Success = false, Message = "This homework is too overdue to extend." };
            if (newDueDate < minD || newDueDate > maxD)
                return new ShopPurchaseResultDto
                {
                    Success = false,
                    Message = $"New due date must be between {minD:dd.MM.yyyy} and {maxD:dd.MM.yyyy}."
                };

            var result = _creditService.PurchaseItem(userId, shopItem.Id);
            if (!result.Success || !result.PurchaseId.HasValue) return result;

            assignment.DueDate = newDueDate;
            _dbContext.SaveChanges();
            _creditService.UpdatePurchaseStatus(result.PurchaseId.Value, "Fulfilled");
            result.Message = $"Deadline extended to {newDueDate:dd.MM.yyyy}!";
            return result;
        }

        public ShopPurchaseResultDto PurchasePointsBoost()
        {
            var userId = _userContextService.GetUserId!.Value;

            var shopItem = _dbContext.ShopItems.FirstOrDefault(x => x.Name == "2× Points Boost" && x.IsActive);

            var purchaseResult = _creditService.PurchaseItem(userId, shopItem.Id);
            if (!purchaseResult.Success || !purchaseResult.PurchaseId.HasValue)
                return purchaseResult;

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId)
                ?? throw new NotFoundException($"User with id: {userId} was not found");

            var today = PolandTime.Today;
            var currentlyActive = user.DoublePointsExpiresAt.HasValue && user.DoublePointsExpiresAt.Value >= today;

            user.DoublePointsExpiresAt = currentlyActive
                ? user.DoublePointsExpiresAt!.Value.AddDays(7)
                : today.AddDays(6);

            _dbContext.SaveChanges();

            _creditService.UpdatePurchaseStatus(purchaseResult.PurchaseId.Value, "Fulfilled");
            purchaseResult.Message = $"Double points active until {user.DoublePointsExpiresAt:dd.MM.yyyy}!";
            return purchaseResult;
        }

        public ShopPurchaseResultDto PurchaseStreakShield()
        {
            var userId = _userContextService.GetUserId!.Value;

            var shopItem = _dbContext.ShopItems.FirstOrDefault(x => x.Name == "Streak Shield" && x.IsActive);
            if (shopItem is null)
                return new ShopPurchaseResultDto { Success = false, Message = "Item 'Streak Shield' is currently unavailable." };

            var purchaseResult = _creditService.PurchaseItem(userId, shopItem.Id);
            if (!purchaseResult.Success || !purchaseResult.PurchaseId.HasValue)
                return purchaseResult;

            _dbContext.UserStreakShields.Add(new UserStreakShield
            {
                UserId = userId,
                IsUsed = false
            });
            _dbContext.SaveChanges();

            _creditService.UpdatePurchaseStatus(purchaseResult.PurchaseId.Value, "Fulfilled");

            var availableCount = _dbContext.UserStreakShields.Count(x => x.UserId == userId && !x.IsUsed);
            purchaseResult.Message = $"Streak Shield purchased! You have {availableCount} available.";
            return purchaseResult;
        }

        private ActionTargetResult CompleteModuleForUser(int userId, int assignmentId)
        {
            if (assignmentId < 0)
            {
                int matrixModuleId = Math.Abs(assignmentId);

                var matrixAssignment = _dbContext.UserMatrixAssignments
                    .FirstOrDefault(x => x.UserId == userId
                                      && x.Matrix.MatrixModules.Any(mm => mm.Id == matrixModuleId));

                if (matrixAssignment is null) return ActionTargetResult.NotFound;

                var alreadyDone = _dbContext.UserMatrixModuleCompletions
                    .Any(x => x.UserId == userId && x.MatrixModuleId == matrixModuleId);

                if (alreadyDone) return ActionTargetResult.AlreadyCompleted;

                _dbContext.UserMatrixModuleCompletions.Add(new UserMatrixModuleCompletion
                {
                    UserId = userId,
                    MatrixModuleId = matrixModuleId,
                    CompletedDate = PolandTime.Today
                });
                _dbContext.SaveChanges();
                return ActionTargetResult.Success;
            }

            var direct = _dbContext.UserModuleAssignments
                .FirstOrDefault(x => x.Id == assignmentId && x.UserId == userId);

            if (direct is null) return ActionTargetResult.NotFound;
            if (direct.IsCompleted) return ActionTargetResult.AlreadyCompleted;

            direct.IsCompleted = true;
            _dbContext.SaveChanges();
            return ActionTargetResult.Success;
        }
    }
}