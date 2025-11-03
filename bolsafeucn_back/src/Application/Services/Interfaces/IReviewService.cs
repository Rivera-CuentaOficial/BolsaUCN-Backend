using bolsafeucn_back.src.Application.DTOs.ReviewDTO;
using bolsafeucn_back.src.Domain.Models;

namespace bolsafeucn_back.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz que define la lógica de negocio para la gestión de reseñas.
    /// Proporciona operaciones para crear, actualizar y consultar reseñas entre oferentes y estudiantes.
    /// </summary>
    public interface IReviewService
    {
        /// <summary>
        /// Agrega la evaluación del oferente hacia el estudiante.
        /// </summary>
        /// <param name="dto">DTO con la información de la reseña del estudiante.</param>
        /// <param name="currentUserId">ID del usuario autenticado (debe ser el oferente).</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="KeyNotFoundException">Lanzada si no se encuentra la reseña.</exception>
        /// <exception cref="UnauthorizedAccessException">Lanzada si el usuario no es el oferente.</exception>
        /// <exception cref="InvalidOperationException">Lanzada si ya completó su evaluación.</exception>
        Task AddStudentReviewAsync(ReviewForStudentDTO dto, int currentUserId);
        
        /// <summary>
        /// Agrega la evaluación del estudiante hacia el oferente.
        /// </summary>
        /// <param name="dto">DTO con la información de la reseña del oferente.</param>
        /// <param name="currentUserId">ID del usuario autenticado (debe ser el estudiante).</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="KeyNotFoundException">Lanzada si no se encuentra la reseña.</exception>
        /// <exception cref="UnauthorizedAccessException">Lanzada si el usuario no es el estudiante.</exception>
        /// <exception cref="InvalidOperationException">Lanzada si ya completó su evaluación.</exception>
        Task AddOfferorReviewAsync(ReviewForOfferorDTO dto, int currentUserId);
        
        /// <summary>
        /// Método para ejecutar acciones cuando ambas partes completan sus reseñas.
        /// Actualmente no implementado.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task BothReviewsCompletedAsync();
        
        /// <summary>
        /// Agrega una nueva reseña completa (obsoleto - no implementado).
        /// </summary>
        /// <param name="dto">DTO con la información de la reseña completa.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="NotImplementedException">Este método no está implementado.</exception>
        Task AddReviewAsync(ReviewDTO dto);
        
        /// <summary>
        /// Obtiene todas las reseñas asociadas a un oferente específico.
        /// </summary>
        /// <param name="offerorId">El identificador del oferente.</param>
        /// <returns>Una colección de DTOs de reseñas del oferente.</returns>
        Task<IEnumerable<ReviewDTO>> GetReviewsByOfferorAsync(int offerorId);
        
        /// <summary>
        /// Calcula el promedio de calificaciones recibidas por un oferente.
        /// </summary>
        /// <param name="offerorId">El identificador del oferente.</param>
        /// <returns>El promedio de calificaciones, o null si no hay reseñas.</returns>
        Task<double?> GetAverageRatingAsync(int offerorId);
        
        /// <summary>
        /// Crea una reseña inicial en estado pendiente para una publicación.
        /// Ambas partes deben completar sus evaluaciones posteriormente.
        /// </summary>
        /// <param name="dto">DTO con los identificadores iniciales de la reseña.</param>
        /// <returns>La reseña inicial creada.</returns>
        /// <exception cref="InvalidOperationException">Lanzada si ya existe una reseña para la publicación.</exception>
        Task<Review> CreateInitialReviewAsync(InitialReviewDTO dto);
        
        /// <summary>
        /// Elimina parcial o completamente una reseña (parte del estudiante, oferente o ambas).
        /// </summary>
        /// <param name="dto">DTO especificando qué partes eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="InvalidOperationException">Lanzada si no se especifica ninguna parte para eliminar.</exception>
        /// <exception cref="KeyNotFoundException">Lanzada si no se encuentra la reseña.</exception>
        Task DeleteReviewPartAsync(DeleteReviewPartDTO dto);
    }
}
