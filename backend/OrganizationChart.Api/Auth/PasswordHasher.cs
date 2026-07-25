using System.Security.Cryptography;

namespace OrganizationChart.Api.Auth;

/// <summary>
/// PBKDF2 (SHA-256) password hashing using only framework primitives.
/// Hash format: "{iterations}.{saltBase64}.{subkeyBase64}".
/// </summary>
public static class PasswordHasher
{
    private const int SaltSize = 16;       // 128-bit salt
    private const int KeySize = 32;        // 256-bit subkey
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var subkey = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        return string.Join('.',
            Iterations,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(subkey));
    }

    public static bool Verify(string password, string hash)
    {
        var parts = hash.Split('.', 3);
        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var expected = Convert.FromBase64String(parts[2]);

        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expected.Length);

        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
