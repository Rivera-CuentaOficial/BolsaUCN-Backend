using bolsafeucn_back.src.Domain.Models;

namespace bolsafeucn_back.src.Application.DTOs.UserDTOs.UserProfileDTOs
{
    public interface IGetUserProfileDTO
    {
        public void ApplyTo(GeneralUser user);
    }
}