using bolsafeucn_back.src.Application.DTOs.ReviewDTO;
using bolsafeucn_back.src.Domain.Models;

namespace bolsafeucn_back.src.Application.Services.Interfaces
{
    public interface IReviewService
    {
        Task AddStudentReviewAsync(ReviewForStudentDTO dto, int currentUserId);
        Task AddOfferorReviewAsync(ReviewForOfferorDTO dto, int currentUserId);
        Task BothReviewsCompletedAsync();
        Task AddReviewAsync(ReviewDTO dto);
        Task<IEnumerable<ReviewDTO>> GetReviewsByOfferorAsync(int offerorId);
        Task<double?> GetAverageRatingAsync(int offerorId);
        Task<Review> CreateInitialReviewAsync(InitialReviewDTO dto);
        Task DeleteReviewPartAsync(DeleteReviewPartDTO dto);
    }
}
