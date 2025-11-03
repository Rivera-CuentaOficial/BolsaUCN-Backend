using bolsafeucn_back.src.Domain.Models;

namespace bolsafeucn_back.src.Infrastructure.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
        Task<IEnumerable<Review>> GetByOfferorIdAsync(int offerorId);
        Task<double?> GetAverageRatingAsync(int providerId);
        Task<Review?> GetByPublicationIdAsync(int publicationId);
        Task<Review?> GetByIdAsync(int reviewId);
        Task UpdateAsync(Review review);
    }
}
