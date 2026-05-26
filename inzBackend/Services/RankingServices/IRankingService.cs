using inzBackend.Models.RankingModels;

namespace inzBackend.Services.RankingServices
{
    public interface IRankingService
    {
        RankingDto getRanking(string period);
        void addReaction(int fromUserId, AddReactionRequest request);
        void removeReaction(int fromUserId, int toUserId, string emoji, string period);
    }
}
