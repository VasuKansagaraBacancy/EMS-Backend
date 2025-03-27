using EMS.EMS.Domain.Entities;

namespace EMS.EMS.Domain.Interfaces
{
    public interface ITokenBlacklistRepository
    {
        Task AddTokenAsync(TokenBlacklist token);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}