using bolsafeucn_back.src.Domain.Models;
using bolsafeucn_back.src.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using bolsafeucn_back.src.Infrastructure.Data;

namespace bolsafeucn_back.src.Infrastructure.Repositories.Implements
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Review>> GetByOfferorIdAsync(int offerorId)
        {
            return await _context.Reviews
                .Where(r => r.OfferorId == offerorId)
                .Include(r => r.Student)
                .ToListAsync();
        }

        public async Task<double?> GetAverageRatingAsync(int offerorId)
        {
            // TODO: Cambiado temporalmente a "OfferorId"
            return await _context.Reviews
                .Where(r => r.OfferorId == offerorId)
                .AverageAsync(r => (double?)r.RatingForOfferor);
        }

        public async Task<Review?> GetByPublicationIdAsync(int publicationId)
        {
            return await _context.Reviews
                .Where(r => r.PublicationId == publicationId)
                .FirstOrDefaultAsync();
        }

        public async Task<Review?> GetByIdAsync(int reviewId)
        {
            return await _context.Reviews
                .Where(r => r.Id == reviewId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }
    }
}
