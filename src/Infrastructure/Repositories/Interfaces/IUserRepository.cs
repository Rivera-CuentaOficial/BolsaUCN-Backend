using bolsafeucn_back.src.Application.DTOs.UserDTOs.AdminDTOs;
using bolsafeucn_back.src.Domain.Models;

namespace bolsafeucn_back.src.Infrastructure.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByRutAsync(string rut);
        Task<bool> GetBlockedStatusAsync(int userId);
        Task<bool> CreateUserAsync(User user, string password, string role);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> UpdateAsync(User user);
        Task<bool> UpdatePasswordAsync(User user, string newPassword);
        Task<bool> UpdateLastLoginAsync(User user);
        Task<string> GetRoleAsync(User user);
        Task<User> GetGeneralUserByIdAsync(int id);
        Task<bool> ConfirmEmailAsync(string email);
        Task<User?> GetUserByIdAsync(
            int userId,
            bool tracking = false,
            bool includePhoto = false,
            bool includeCV = false
        );
        Task<(IEnumerable<User>, int TotalCount)> GetFilteredForAdminAsync(
            int adminId,
            SearchParamsDTO searchParams
        );
        Task<int> GetNumberOfAdmins();
        Task<bool> DeleteUserAsync(User user);
    }
}
