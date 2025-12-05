using bolsafeucn_back.src.Domain.Models;
using bolsafeucn_back.src.Infrastructure.Data;
using bolsafeucn_back.src.Infrastructure.Repositories.Interfaces;

namespace bolsafeucn_back.src.Infrastructure.Repositories.Implements
{
    public class TokenRepository : ITokenRepository
    {
        private readonly AppDbContext _context;
        public TokenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Whitelist> AddToWhitelistAsync(Whitelist whitelistEntry)
        {
            await _context.Whitelists.AddAsync(whitelistEntry);
            await _context.SaveChangesAsync();
            return whitelistEntry;
        }
        
        public async Task<bool> RemoveFromWhitelistAsync(int userId)
        {
            var tokens = _context.Whitelists.Where(w => w.UserId == userId);
            _context.Whitelists.RemoveRange(tokens);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<Blacklist> AddToBlacklistAsync(Blacklist blacklistEntry)
        {
            await _context.Blacklists.AddAsync(blacklistEntry);
            await _context.SaveChangesAsync();
            return blacklistEntry;
        }

        public async Task<bool> RemoveFromBlacklistAsync(int userId)
        {
            var tokens = _context.Blacklists.Where(b => b.UserId == userId);
            _context.Blacklists.RemoveRange(tokens);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<Whitelist>> GetAllByUserIdAsync(int userId)
        {
            return _context.Whitelists.Where(w => w.UserId == userId);
        }
    }
}