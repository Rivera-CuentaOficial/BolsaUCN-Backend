using bolsafeucn_back.src.Application.DTOs.ReviewDTO;
using bolsafeucn_back.src.Domain.Models;

namespace bolsafeucn_back.src.Application.Mappers
{
    /// <summary>
    /// Mapper para transformar entre entidades de dominio Review y sus DTOs correspondientes.
    /// Proporciona métodos para mapear datos en ambas direcciones y actualizar entidades existentes.
    /// </summary>
    public static class ReviewMapper
    {
        /// <summary>
        /// Actualiza una reseña existente con la evaluación del oferente hacia el estudiante.
        /// Marca la evaluación del oferente como completada.
        /// </summary>
        /// <param name="dto">DTO con la calificación y comentarios del oferente para el estudiante.</param>
        /// <param name="review">La entidad de reseña a actualizar.</param>
        /// <returns>La reseña actualizada con los datos del oferente.</returns>
        public static Review studentUpdateReview(ReviewForStudentDTO dto, Review review)
        {
            review.RatingForStudent = dto.RatingForStudent;
            review.CommentForStudent = dto.CommentForStudent;
            review.AtTime = dto.atTime;
            review.GoodPresentation = dto.goodPresentation;
            review.StudentReviewCompleted = true;
            review.IsCompleted = review.StudentReviewCompleted && review.OfferorReviewCompleted;
            return review;
        }

        /// <summary>
        /// Actualiza una reseña existente con la evaluación del estudiante hacia el oferente.
        /// Marca la evaluación del estudiante como completada.
        /// </summary>
        /// <param name="dto">DTO con la calificación y comentarios del estudiante para el oferente.</param>
        /// <param name="review">La entidad de reseña a actualizar.</param>
        /// <returns>La reseña actualizada con los datos del estudiante.</returns>
        public static Review offerorUpdateReview(ReviewForOfferorDTO dto, Review review)
        {
            review.RatingForOfferor = dto.RatingForOfferor;
            review.CommentForOfferor = dto.CommentForOfferor;
            review.OfferorReviewCompleted = true;
            review.IsCompleted = review.StudentReviewCompleted && review.OfferorReviewCompleted;
            return review;
        }

        /// <summary>
        /// Crea una reseña inicial en estado pendiente a partir de un DTO inicial.
        /// Establece automáticamente la ventana de revisión en 14 días desde la creación.
        /// Las evaluaciones deben completarse posteriormente por ambas partes.
        /// </summary>
        /// <param name="dto">DTO con los identificadores del estudiante, oferente y publicación.</param>
        /// <returns>Una nueva entidad Review con estado inicial y ventana de revisión de 14 días.</returns>
        public static Review CreateInitialReviewAsync(InitialReviewDTO dto, GeneralUser student, GeneralUser offeror)
        {
            return new Review
            {
                StudentId = dto.StudentId,
                Student = student,
                OfferorId = dto.OfferorId,
                Offeror = offeror,
                PublicationId = dto.PublicationId
            };
        }
        // TODO: Revisar si es necesario este método
        // public static Review ToEntity(ReviewDTO dto)
        // {
        //     return new Review
        //     {
        //         Rating = dto.Rating,
        //         Comment = dto.Comment,
        //         StudentId = dto.StudentId,
        //         ProviderId = dto.ProviderId
        //     };
        // }
        /// <summary>
        /// Convierte un DTO de reseña a una entidad Review del dominio.
        /// </summary>
        /// <param name="dto">El DTO de reseña a convertir.</param>
        /// <returns>Una entidad Review con los datos del DTO.</returns>

        public static ReviewDTO ToDTO(Review entity)
        {
            return new ReviewDTO
            {
                idReview = entity.Id,
                RatingForStudent = entity.RatingForStudent,
                CommentForStudent = entity.CommentForStudent,
                RatingForOfferor = entity.RatingForOfferor,
                CommentForOfferor = entity.CommentForOfferor,
                AtTime = entity.AtTime,
                GoodPresentation = entity.GoodPresentation,
                ReviewWindowEndDate = entity.ReviewWindowEndDate,
                IdStudent = entity.StudentId,
                IdOfferor = entity.OfferorId,
                IdPublication = entity.PublicationId,
                HasReviewForOfferorBeenDeleted = entity.HasReviewForOfferorBeenDeleted,
                HasReviewForStudentBeenDeleted = entity.HasReviewForStudentBeenDeleted,
                IsComplete = entity.IsCompleted
            };
        }
        public static ShowReviewDTO ShowReviewDTO(Review entity)
        {
            return new ShowReviewDTO
            {
                idReview = entity.Id,
                RatingForStudent = entity.RatingForStudent ?? 0,
                CommentForStudent = entity.CommentForStudent ?? string.Empty,
                RatingForOfferor = entity.RatingForOfferor ?? 0,
                CommentForOfferor = entity.CommentForOfferor ?? string.Empty,
                AtTime = entity.AtTime,
                GoodPresentation = entity.GoodPresentation,
                IsComplete = entity.IsCompleted
            };
        }
    }
}

