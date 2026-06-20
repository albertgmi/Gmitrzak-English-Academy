using inzBackend.Models.RankingModels;

namespace inzBackend.Services.RankingServices
{
    public interface IRankingService
    {
        RankingDto GetRanking(string period);
        void AddReaction(AddReactionRequest request);
        void RemoveReaction(int toUserId, string emoji, string period);
    }
}
