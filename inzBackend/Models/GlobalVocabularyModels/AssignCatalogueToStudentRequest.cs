namespace inzBackend.Models.GlobalVocabularyModels
{
    public class AssignCatalogueToStudentRequest
    {
        public int StudentUserId { get; set; }
        public int CatalogueId { get; set; }
    }
}
