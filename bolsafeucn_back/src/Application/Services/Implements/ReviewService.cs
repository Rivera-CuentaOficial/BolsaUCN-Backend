using bolsafeucn_back.src.Application.DTOs.ReviewDTO;
using bolsafeucn_back.src.Application.Mappers;
using bolsafeucn_back.src.Application.Services.Interfaces;
using bolsafeucn_back.src.Domain.Models;
using bolsafeucn_back.src.Infrastructure.Repositories.Interfaces;
using Serilog;

namespace bolsafeucn_back.src.Application.Services.Implements
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repository;

        public ReviewService(IReviewRepository repository)
        {
            _repository = repository;
        }

        public async Task AddReviewAsync(ReviewDTO dto)
        {
            // var review = ReviewMapper.ToEntity(dto);
            // await _repository.AddAsync(review);
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByOfferorAsync(int offerorId)
        {
            var reviews = await _repository.GetByOfferorIdAsync(offerorId);
            return reviews.Select(ReviewMapper.ToDTO);
        }

        public async Task<double?> GetAverageRatingAsync(int offerorId)
        {
            return await _repository.GetAverageRatingAsync(offerorId);
        }

        public async Task AddStudentReviewAsync(ReviewForStudentDTO dto, int currentUserId)
        {
            var review = await _repository.GetByPublicationIdAsync(dto.PublicationId);
            if(review == null)
                throw new KeyNotFoundException("No se ha encontrado una reseña para el ID de publicación dado.");
            
            // Validar que el usuario actual sea el OFERENTE (quien califica al estudiante)
            if(review.OfferorId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el oferente de esta publicación puede dejar una review hacia el estudiante.");
            }

            // Validar que el oferente no haya completado ya su review hacia el estudiante
            if(review.OfferorReviewCompleted)
            {
                throw new InvalidOperationException("Ya has completado tu review para este estudiante.");
            }

            ReviewMapper.studentUpdateReview(dto, review);
            if(review.StudentReviewCompleted) {
                review.IsCompleted = true;
            }
            await _repository.UpdateAsync(review);
            Log.Information("Offeror {OfferorId} added review for student in publication {PublicationId}", currentUserId, dto.PublicationId);
        }

        public async Task AddOfferorReviewAsync(ReviewForOfferorDTO dto, int currentUserId)
        {
            var review = await _repository.GetByPublicationIdAsync(dto.PublicationId);
            if(review == null)
                throw new KeyNotFoundException("No se ha encontrado una reseña para el ID de publicación dado.");
            
            // Validar que el usuario actual sea el ESTUDIANTE (quien califica al oferente)
            if(review.StudentId != currentUserId)
            {
                throw new UnauthorizedAccessException("Solo el estudiante de esta publicación puede dejar una review hacia el oferente.");
            }

            // Validar que el estudiante no haya completado ya su review hacia el oferente
            if(review.StudentReviewCompleted)
            {
                throw new InvalidOperationException("Ya has completado tu review para este oferente.");
            }

            ReviewMapper.offerorUpdateReview(dto, review);
            if(review.OfferorReviewCompleted) {
                review.IsCompleted = true;
                //await BothReviewsCompletedAsync();
            }
            await _repository.UpdateAsync(review);
            Log.Information("Student {StudentId} added review for offeror in publication {PublicationId}", currentUserId, dto.PublicationId);
        }

        public Task BothReviewsCompletedAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Review> CreateInitialReviewAsync(InitialReviewDTO dto)
        {
            // Validar que no exista ya una review para esta publicación
            var existingReview = await _repository.GetByPublicationIdAsync(dto.PublicationId);
            if (existingReview != null)
            {
                throw new InvalidOperationException($"Ya existe una review para la publicación con ID {dto.PublicationId}.");
            }
            var review = ReviewMapper.CreateInitialReviewAsync(dto);
            await _repository.AddAsync(review);
            return review;
        }

        public async Task DeleteReviewPartAsync(DeleteReviewPartDTO dto)
        {
            // Validar que se solicite eliminar al menos una parte
            if (!dto.DeleteStudentPart && !dto.DeleteOfferorPart)
            {
                throw new InvalidOperationException("Debe especificar al menos una parte de la review para eliminar.");
            }

            // Obtener la review
            var review = await _repository.GetByIdAsync(dto.ReviewId);
            if (review == null)
            {
                throw new KeyNotFoundException($"No se encontró una review con ID {dto.ReviewId}.");
            }

            // Eliminar la parte del estudiante si se solicita
            if (dto.DeleteStudentPart)
            {
                review.RatingForOfferor = null;
                review.CommentForOfferor = null;
                review.AtTime = false;
                review.GoodPresentation = false;
                review.StudentReviewCompleted = false;
                Log.Information("Deleted student part of review ID {ReviewId}", dto.ReviewId);
            }

            // Eliminar la parte del oferente si se solicita
            if (dto.DeleteOfferorPart)
            {
                review.RatingForStudent = null;
                review.CommentForStudent = null;
                review.OfferorReviewCompleted = false;
                Log.Information("Deleted offeror part of review ID {ReviewId}", dto.ReviewId);
            }

            // Si se eliminaron ambas partes, marcar la review como no completada
            if (dto.DeleteStudentPart && dto.DeleteOfferorPart)
            {
                review.IsCompleted = false;
            }
            // Si solo queda una parte completada, mantener IsCompleted como false
            else if (!review.StudentReviewCompleted || !review.OfferorReviewCompleted)
            {
                review.IsCompleted = false;
            }

            await _repository.UpdateAsync(review);
        }
    }
}
