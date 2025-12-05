using bolsafeucn_back.src.Domain.Models;

namespace bolsafeucn_back.src.Infrastructure.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Task<Whitelist> AddToWhitelistAsync(Whitelist token);
        Task<bool> RemoveFromWhitelistAsync(int userId);
        Task<Blacklist> AddToBlacklistAsync(Blacklist token);
        Task<bool> RemoveFromBlacklistAsync(int userId);
        Task<IEnumerable<Whitelist>> GetAllByUserIdAsync(int userId);
    }
}