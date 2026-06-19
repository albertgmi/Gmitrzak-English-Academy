using inzBackend.Entities.Assignments;
using inzBackend.Enums;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.CreditModels;
using inzBackend.Services.CreditServices;

namespace inzBackend.Services.ShopActionServices
{
    public class ShopActionService : IShopActionService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly ICreditService _creditService;

        public ShopActionService(GmitrzakEnglishAcademyDbContext dbContext, ICreditService creditService)
        {
            _dbContext = dbContext;
            _creditService = creditService;
        }
        public ShopPurchaseResultDto SkipHomework(int userId, int assignmentId)
        {
            var shopItem = _dbContext.ShopItems.FirstOrDefault(x => x.Name == "Homework Skip" && x.IsActive);
            if (shopItem is null)
                return new ShopPurchaseResultDto { Success = false, Message = "Item 'Homework Skip' is currently unavailable." };

            var purchaseResult = _creditService.purchaseItem(userId, shopItem.Id);
            if (!purchaseResult.Success || !purchaseResult.PurchaseId.HasValue)
                return purchaseResult;

            var actionResult = CompleteModuleForUser(userId, assignmentId);
            if (actionResult == ActionTargetResult.Success)
            {
                _creditService.updatePurchaseStatus(purchaseResult.PurchaseId.Value, "Fulfilled");
                purchaseResult.Message = "Homework skipped successfully!";
                return purchaseResult;
            }

            _creditService.updatePurchaseStatus(purchaseResult.PurchaseId.Value, "Cancelled");
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