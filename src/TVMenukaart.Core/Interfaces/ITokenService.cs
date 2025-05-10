using TVMenukaart.Models;

namespace TVMenukaart.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser appUser);
        RefreshToken GenerateRefreshToken();
    }
}
