using Application.Data.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<string> _passwordHasher = new();

        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(string.Empty, password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return _passwordHasher.VerifyHashedPassword(string.Empty, passwordHash, password) != PasswordVerificationResult.Failed;
        }
    }
}
