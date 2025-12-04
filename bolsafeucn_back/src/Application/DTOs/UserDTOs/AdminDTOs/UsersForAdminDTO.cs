using bolsafeucn_back.src.Application.DTOs.UserDTOs.UserProfileDTOs;

namespace bolsafeucn_back.src.Application.DTOs.UserDTOs.AdminDTOs
{
    public class UsersForAdminDTO
    {
        public List<IGetUserProfileDTO> Users { get; set; } = new List<IGetUserProfileDTO>();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}