using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.EMS.Infrastructure.Repositories
{
    public class PasswordResetTokenRepo : IPasswordResetTokenRepo
    {
        private readonly EMSDbContext _context;

        public PasswordResetTokenRepo(EMSDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            try
            {
                await _context.PasswordResetTokens.AddAsync(token);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database error while adding a password reset token.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while adding a password reset token.", ex);
            }
        }
        public async Task<PasswordResetToken?> GetByTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token must be provided.", nameof(token));

            try
            {
                return await _context.PasswordResetTokens
                    .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiryDate > DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the password reset token.", ex);
            }
        }
        public async Task UpdateAsync(PasswordResetToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            try
            {
                _context.PasswordResetTokens.Update(token);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database error while updating the password reset token.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating the password reset token.", ex);
            }
        }
    }
}
