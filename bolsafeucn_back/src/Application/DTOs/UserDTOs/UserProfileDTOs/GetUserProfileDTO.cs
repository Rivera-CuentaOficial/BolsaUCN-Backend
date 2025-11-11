namespace bolsafeucn_back.src.Application.DTOs.UserDTOs.UserProfileDTOs
{
    public class GetUserProfileDTO
    {
        public required string UserName { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }      
        public string? CompanyName { get; set; }
        public string? LegalName{ get; set; }  
        public required string Rut { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; } 
        public float? Rating { get; set; }
        public required string AboutMe { get; set; }
        public string? CurriculumVitae { get; set; }
        //public required ICollection<string> MotivationalLetters { get; set; }
    }
}