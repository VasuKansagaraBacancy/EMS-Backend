using EMS.EMS.Domain.Entities;

namespace EMS.EMS.Domain.Interfaces
{
    public interface IPasswordResetTokenRepo
    {
        Task AddAsync(PasswordResetToken token);
        Task<PasswordResetToken> GetByTokenAsync(string token);
        Task UpdateAsync(PasswordResetToken token);
    }

}
