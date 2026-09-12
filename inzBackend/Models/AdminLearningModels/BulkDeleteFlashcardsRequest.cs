using System.Collections.Generic;

namespace inzBackend.Models.AdminLearningModels
{
    public class BulkDeleteFlashcardsRequest
    {
        public List<int> FlashcardIds { get; set; } = new List<int>();
    }
}
