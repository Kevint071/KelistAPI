using ErrorOr;

namespace Domain.DomainErrors
{
    public static partial class Errors
    {
        public static class User
        {
            public static readonly Error NotFound = Error.NotFound("User.NotFound", "The user with the provided Id was not found.");
            public static readonly Error DuplicatedEmail = Error.Conflict("User.DuplicateEmail", "El correo ya está registrado.");
            public static readonly Error InvalidCredentials = Error.Unauthorized("User.InvalidCredentials", "Credenciales inválidas.");
            public static readonly Error InvalidRefreshToken = Error.Unauthorized("User.InvalidRefreshToken", "Refresh token inválido o expirado.");
        }
    }
}
