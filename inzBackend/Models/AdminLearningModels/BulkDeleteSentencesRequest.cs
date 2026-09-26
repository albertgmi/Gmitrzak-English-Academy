using System.Collections.Generic;

namespace inzBackend.Models.AdminLearningModels
{
    public class BulkDeleteSentencesRequest
    {
        public List<int> SentenceIds { get; set; } = new List<int>();
    }
}
