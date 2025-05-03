using RemoteMenu.Models;

namespace RemoteMenu.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser appUser);
        RefreshToken GenerateRefreshToken();
    }
}
