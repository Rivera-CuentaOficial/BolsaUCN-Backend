namespace bolsafeucn_back.src.Application.DTOs.ReviewDTO
{
    public class DeleteReviewPartDTO
    {
        public required int ReviewId { get; set; }
        public bool DeleteStudentPart { get; set; } = false;
        public bool DeleteOfferorPart { get; set; } = false;
    }
}
