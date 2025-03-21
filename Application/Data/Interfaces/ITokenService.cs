using System.Security.Claims;

namespace Application.Data.Interfaces
{
    public interface ITokenService
    {
        string CreateJwtToken(IEnumerable<Claim> claims); // Cambiado para aceptar claims en lugar de User
        string GenerateRefreshToken();
    }
}
