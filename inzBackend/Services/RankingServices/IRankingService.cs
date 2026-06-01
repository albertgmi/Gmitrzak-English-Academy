using inzBackend.Models.RankingModels;

namespace inzBackend.Services.RankingServices
{
    public interface IRankingService
    {
        RankingDto getRanking(string period);
        void addReaction(AddReactionRequest request);
        void removeReaction(int toUserId, string emoji, string period);
    }
}
