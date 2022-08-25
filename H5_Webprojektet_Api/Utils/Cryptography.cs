using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace H5_Webprojektet_Api.Utils;

public class Cryptography
{
    public static byte[] CreateSalt(int saltSize, RandomNumberGenerator rng)
    {
        var salt = new byte[saltSize];
        rng.GetBytes(salt);

        return salt;
    }

    public static (string, string) HashPasswordV2(string password, RandomNumberGenerator rng)
    {
        const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
        const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
        const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
        const int SaltSize = 128 / 8; // 128 bits

        var salt = CreateSalt(SaltSize, rng);
        var subkey = KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

        var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
        outputBytes[0] = 0x00; // format marker
        Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
        Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
        return (Convert.ToBase64String(outputBytes), Convert.ToBase64String(salt));
    }

    public static bool VerifyHashedPassword(string hashedPassword, string providedSalt, string providedPassword)
    {
        var password = Convert.FromBase64String(hashedPassword);
        var salt = Convert.FromBase64String(providedSalt);
        const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
        const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
        const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
        const int SaltSize = 128 / 8; // 128 bits

        var expectedSubkey = new byte[Pbkdf2SubkeyLength];
        Buffer.BlockCopy(password, 1 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

        // Hash the password and verify it
        var actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);
        return ByteArraysEqual(actualSubkey, expectedSubkey);
    }

    // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static bool ByteArraysEqual(byte[] a, byte[] b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null || a.Length != b.Length)
        {
            return false;
        }

        var isSame = true;
        for (var i = 0; i < a.Length; i++)
        {
            isSame &= (a[i] == b[i]);
        }

        return isSame;
    }

    public static string EncryptString(string text, string key)
    {
        var iv = new byte[16];
        byte[] array;

        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (var streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(text);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    public static string DecryptString(string encryptedText, string key)
    {
        var iv = new byte[16];
        var buffer = Convert.FromBase64String(encryptedText);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var memoryStream = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        return streamReader.ReadToEnd();
    }
}