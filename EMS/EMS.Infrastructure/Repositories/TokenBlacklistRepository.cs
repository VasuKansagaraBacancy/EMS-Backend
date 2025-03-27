using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.EMS.Infrastructure.Repositories
{
    public class TokenBlacklistRepository : ITokenBlacklistRepository
    {
        private readonly EMSDbContext _context;
        public TokenBlacklistRepository(EMSDbContext context)
        {
            _context = context;
        }
        public async Task AddTokenAsync(TokenBlacklist token)
        {
            try
            {
                _context.TokenBlacklists.Add(token);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the token to the blacklist.", ex);
            }
        }
        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            try
            {
                return await _context.TokenBlacklists.AnyAsync(t => t.Token == token);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while checking if the token is blacklisted.", ex);
            }
        }
    }
}