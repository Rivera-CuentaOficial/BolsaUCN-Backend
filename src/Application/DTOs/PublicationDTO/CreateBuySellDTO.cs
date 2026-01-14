using System.ComponentModel.DataAnnotations;

namespace bolsafeucn_back.src.Application.DTOs.PublicationDTO
{
    /// <summary>
    /// DTO para la creación de publicaciones de compra/venta
    /// </summary>
    public class CreateBuySellDTO
    {
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(
            200,
            MinimumLength = 5,
            ErrorMessage = "El título debe tener entre 5 y 200 caracteres"
        )]
        public required string Title { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(
            2000,
            MinimumLength = 10,
            ErrorMessage = "La descripción debe tener entre 10 y 2000 caracteres"
        )]
        public required string Description { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(100, ErrorMessage = "La categoría no puede exceder 100 caracteres")]
        public required string Category { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, 100000000, ErrorMessage = "El precio debe estar entre $0 y $100.000.000")]
        public required decimal Price { get; set; }

        /* === IMAGENES (NO USADO POR AHORA) ===
        public required List<IFormFile> Images { get; set; } = new List<IFormFile>();
        */

        [StringLength(200, ErrorMessage = "La ubicación no puede exceder 200 caracteres")]
        public string? Location { get; set; }

        [StringLength(
            200,
            ErrorMessage = "La información de contacto no puede exceder 200 caracteres"
        )]
        public string? ContactInfo { get; set; }
    }
}
