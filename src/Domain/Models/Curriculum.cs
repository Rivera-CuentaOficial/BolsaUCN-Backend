namespace bolsafeucn_back.src.Domain.Models
{
    public class Curriculum : ModelBase
    {
        // === METADATA DEL ARCHIVO ===
        public required string OriginalFileName { get; set; }
        public long FileSizeBytes { get; set; }

        // === STORAGE ===
        public required string Url { get; set; }
        public required string PublicId { get; set; }

        // === AUDITORÍA ===
        public bool IsActive { get; set; } = true;
    }
}
