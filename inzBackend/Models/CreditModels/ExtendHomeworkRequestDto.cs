namespace inzBackend.Models.CreditModels
{
    public class ExtendHomeworkRequestDto
    {
        public int ShopItemId { get; set; }
        public int AssignmentId { get; set; }
        public DateTime NewDueDate { get; set; }
    }
}
