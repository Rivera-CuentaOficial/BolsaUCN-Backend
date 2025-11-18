namespace bolsafeucn_back.src.Application.DTOs.JobAplicationDTO
{
    public class CreateJobApplicationDto
    {
        public int JobOfferId { get; set; }
        public string? MotivationLetter { get; set; }
    }

    public class JobApplicationResponseDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string OfferTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ApplicationDate { get; set; }
        public string? CurriculumVitae { get; set; }
        public string? MotivationLetter { get; set; }
    }

    public class JobApplicationDetailDto
    {
        // Datos de la postulación
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ApplicationDate { get; set; }
        public string? MotivationLetter { get; set; }

        // Datos completos de la oferta
        public int OfferId { get; set; }
        public string OfferTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Requirements { get; set; }
        public string? OfferType { get; set; }
        public int Remuneration { get; set; }
        public string? Location { get; set; }
        public DateTime? DeadlineDate { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactInfo { get; set; }
    
    }
}
