using Microsoft.AspNetCore.Identity;

namespace MiniCRMProject.Services
{
    // Şifreleri düz metin yerine tuzlanmış ve karıştırılmış (hash) olarak saklar.
    // .NET'in hazır PasswordHasher sınıfını kullanır (PBKDF2).
    public class PasswordService
    {
        private static readonly object HashUser = new();
        private readonly PasswordHasher<object> hasher = new();

        public string Hash(string password)
        {
            return hasher.HashPassword(HashUser, password);
        }

        public bool Verify(string hash, string password)
        {
            try
            {
                PasswordVerificationResult result = hasher.VerifyHashedPassword(HashUser, hash, password);
                return result != PasswordVerificationResult.Failed;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
