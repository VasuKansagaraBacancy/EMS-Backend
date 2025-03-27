using EMS.EMS.Domain.Entities;

namespace EMS.EMS.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}