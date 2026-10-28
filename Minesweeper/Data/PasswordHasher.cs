using System.Security.Cryptography;

namespace Minesweeper.Data;

public static class PasswordHasher
{
    // Создаёт соль и хеш пароля.
    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: 600_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32);

        // Храним соль и хеш вместе, в текстовом формате.
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    // Проверяет введённый пароль по сохранённому хешу.
    public static bool VerifyPassword(string password, string storedPassword)
    {
        string[] parts = storedPassword.Split(':');

        if (parts.Length != 2)
            return false;

        try
        {
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedHash = Convert.FromBase64String(parts[1]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: 600_000,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: 32);

            return CryptographicOperations.FixedTimeEquals(
                actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}