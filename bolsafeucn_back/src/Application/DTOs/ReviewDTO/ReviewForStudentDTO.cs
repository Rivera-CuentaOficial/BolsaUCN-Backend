using System.ComponentModel.DataAnnotations;
using bolsafeucn_back.src.Domain.Models;

namespace bolsafeucn_back.src.Application.DTOs.ReviewDTO
{
    public class ReviewForStudentDTO
    {
        [Required(ErrorMessage = "El rating es obligatorio.")]
        [Range(1, 6, ErrorMessage = "El rating debe tener entre 1 y 6 estrellas como valores enteros.")]
        public int RatingForStudent { get; set; }
        [Required(ErrorMessage = "El comentario es obligatorio.")]
        [StringLength(320, ErrorMessage = "El comentario no puede exceder los 320 caracteres.")]
        public string CommentForStudent { get; set; } = string.Empty;
        [Required(ErrorMessage = "La fecha de envío es obligatoria.")]
        public DateTime SendedAt { get; set; }
        public required bool atTime { get; set; }
        public required bool goodPresentation { get; set; }
        public required int PublicationId { get; set; }
    }
}