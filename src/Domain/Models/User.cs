using Microsoft.AspNetCore.Identity;

namespace bolsafeucn_back.src.Domain.Models
{
    public enum UserType
    {
        Estudiante,
        Empresa,
        Particular,
        Administrador,
    }

    public enum Disability
    {
        Ninguna,
        Visual,
        Auditiva,
        Motriz,
        Cognitiva,
        Otra,
    }

    public class User : IdentityUser<int>
    {
        // === PROPIEDADES GENERALES ===
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required UserType UserType { get; set; }
        public required string Rut { get; set; }
        public string AboutMe { get; set; } = string.Empty;
        public double Rating { get; set; } = 6.0;

        // === PROPIEDADES DE ESTUDIANTES ===
        public int? CVId { get; set; }
        public Curriculum? CV { get; set; }
        public Disability? Disability { get; set; }

        // === PROPIEDADES DE AUDITORIA ===
        public required bool IsBanned { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        
        // === PUBLICACIONES ===
        public ICollection<Publication> Publications { get; set; } = new List<Publication>();

        // === IMÁGENES ===
        public int? ProfilePhotoId { get; set; }
        public UserImage? ProfilePhoto { get; set; } = null;
    }
}