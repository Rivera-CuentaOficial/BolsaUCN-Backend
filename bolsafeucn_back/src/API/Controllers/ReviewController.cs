using bolsafeucn_back.src.Application.DTOs.ReviewDTO;
using bolsafeucn_back.src.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bolsafeucn_back.src.API.Controllers
{
    [ApiController]
    [Route("reviews/[action]")] // [action] es el nombre de la funcion.
    /// <summary>
    /// Controlador para gestionar las reseñas entre oferentes y estudiantes.
    /// </summary>
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        /// <summary>
        /// Agrega una nueva reseña.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewDTO dto)
        {
            await _reviewService.AddReviewAsync(dto);
            return Ok("Review added successfully");
        }
        /// <summary>
        /// Obtiene todas las reseñas para un oferente específico.
        /// </summary>
        /// <param name="offerorId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetReviews(int offerorId)
        {
            var reviews = await _reviewService.GetReviewsByOfferorAsync(offerorId);
            return Ok(reviews);
        }
        /// <summary>
        /// Obtiene la calificación promedio de un oferente específico.
        /// </summary>
        /// <param name="offerorId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAverage(int offerorId)
        {
            var avg = await _reviewService.GetAverageRatingAsync(offerorId);
            return Ok(avg);
        }
        /// <summary>
        /// Agrega la reseña del oferente hacia el estudiante.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Offerent")]
        public async Task<IActionResult> AddStudentReview([FromBody] ReviewForStudentDTO dto)
        {
            // Obtener el ID del usuario autenticado (OFERENTE calificando al estudiante)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            await _reviewService.AddStudentReviewAsync(dto, currentUserId);
            return Ok("Student review added successfully");
        }
        /// <summary>
        /// Agrega la reseña del estudiante hacia el oferente.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Applicant")]
        public async Task<IActionResult> AddOfferorReview([FromBody] ReviewForOfferorDTO dto)
        {
            // Obtener el ID del usuario autenticado (ESTUDIANTE calificando al oferente)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            await _reviewService.AddOfferorReviewAsync(dto, currentUserId);
            return Ok("Offeror review added successfully");
        }
        /// <summary>
        /// Agrega una nueva reseña inicial.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddInitialReview([FromBody] InitialReviewDTO dto)
        {
            await _reviewService.CreateInitialReviewAsync(dto);
            return Ok("Initial review added successfully");
        }
        /// <summary>
        /// Elimina una parte de la reseña.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReviewPart([FromBody] DeleteReviewPartDTO dto)
        {
            await _reviewService.DeleteReviewPartAsync(dto);
            return Ok("Review part(s) deleted successfully");
        }
    }
}