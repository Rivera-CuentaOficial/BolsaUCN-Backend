namespace bolsafeucn_back.src.Application.DTOs.UserDTOs.AdminDTOs
{
    public class UserForAdminDTO
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Rut { get; set; }
        public required string PhoneNumber { get; set; }
        public required string UserType { get; set; }
        public required float Rating { get; set; }
        public required bool IsBlocked { get; set; }
    }
}