namespace inzBackend.Models.ExaminationModels
{
    public class ExaminationDto
    {
        public List<ExaminationFlashcardDto> Flashcards { get; set; } = [];
        public List<ExaminationSentenceDto> Sentences { get; set; } = [];
        public List<ExaminationMemoryDto> Memories { get; set; } = [];
    }
}
