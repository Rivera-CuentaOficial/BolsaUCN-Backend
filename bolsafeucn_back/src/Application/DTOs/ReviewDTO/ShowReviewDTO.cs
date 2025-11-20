namespace bolsafeucn_back.src.Application.DTOs.ReviewDTO
{
    public class ShowReviewDTO
    {
        public required int idReview { get; set; }
        public required int RatingForStudent { get; set; }
        public required string CommentForStudent { get; set; }
        public required int RatingForOfferor { get; set; }
        public required string CommentForOfferor { get; set; }
        public required bool AtTime { get; set; }
        public required bool GoodPresentation { get; set; }
        public required bool IsComplete { get; set; }
    }
}