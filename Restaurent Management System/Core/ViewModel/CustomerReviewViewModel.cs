namespace PMSCore.ViewModel
{
    public class CustomerReviewViewModel
    {
        public int CustomerId { get; set; } = 0;
        public int OrderId { get; set; } = 0;
        public short FoodRating { get; set; } = 0;
        public short ServiceRating { get; set; } = 0;
        public short AmbienceRating { get; set; } = 0;
        public string Comment { get; set; } = string.Empty;
    }
}

