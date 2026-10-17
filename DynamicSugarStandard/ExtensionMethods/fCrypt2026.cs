using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Password-based string encryption.
///  - Key derivation: PBKDF2 (HMAC-SHA256) with a random per-message salt
///  - Encryption:     AES-256-CBC with a random IV
///  - Integrity:      HMAC-SHA256 over salt + IV + ciphertext (Encrypt-then-MAC)
/// Output format (Base64): [salt 16][iv 16][ciphertext n][hmac 32]
/// </summary>
public static class StringCrypt2026
{
    private const int SaltSize = 16;
    private const int KeySize = 32; // 256-bit
    private const int IvSize = 16; // 128-bit
    private const int Iterations = 100_000;
    private const int HmacSize = 32; // HMAC-SHA256 tag

    public static string Encrypt(string plainText, string password)
    {
        if (plainText == null) throw new ArgumentNullException(nameof(plainText));
        if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password must not be empty.", nameof(password));

        byte[] salt = new byte[SaltSize];
        byte[] iv = new byte[IvSize];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
            rng.GetBytes(iv);
        }

        DeriveKeys(password, salt, out byte[] encKey, out byte[] macKey);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        try
        {
            byte[] cipherBytes;
            using (Aes aes = CreateAes(encKey, iv))
            using (ICryptoTransform encryptor = aes.CreateEncryptor())
            {
                cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            }

            int dataLength = SaltSize + IvSize + cipherBytes.Length;
            byte[] result = new byte[dataLength + HmacSize];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(iv, 0, result, SaltSize, IvSize);
            Buffer.BlockCopy(cipherBytes, 0, result, SaltSize + IvSize, cipherBytes.Length);

            using (var hmac = new HMACSHA256(macKey))
            {
                byte[] tag = hmac.ComputeHash(result, 0, dataLength);
                Buffer.BlockCopy(tag, 0, result, dataLength, HmacSize);
            }

            return Convert.ToBase64String(result);
        }
        finally
        {
            Array.Clear(plainBytes, 0, plainBytes.Length);
            Array.Clear(encKey, 0, encKey.Length);
            Array.Clear(macKey, 0, macKey.Length);
        }
    }

    public static string Decrypt(string cipherText, string password)
    {
        if (cipherText == null) throw new ArgumentNullException(nameof(cipherText));
        if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password must not be empty.", nameof(password));

        byte[] data;
        try
        {
            data = Convert.FromBase64String(cipherText);
        }
        catch (FormatException)
        {
            throw new CryptographicException("Decryption failed.");
        }

        // Minimum: salt + IV + one AES block + HMAC
        if (data.Length < SaltSize + IvSize + IvSize + HmacSize)
            throw new CryptographicException("Decryption failed.");

        byte[] salt = new byte[SaltSize];
        byte[] iv = new byte[IvSize];
        Buffer.BlockCopy(data, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(data, SaltSize, iv, 0, IvSize);

        int dataLength = data.Length - HmacSize;
        int cipherLength = dataLength - SaltSize - IvSize;

        DeriveKeys(password, salt, out byte[] encKey, out byte[] macKey);
        try
        {
            // Verify integrity BEFORE decrypting (prevents padding-oracle attacks)
            using (var hmac = new HMACSHA256(macKey))
            {
                byte[] expectedTag = hmac.ComputeHash(data, 0, dataLength);
                if (!ConstantTimeEquals(expectedTag, 0, data, dataLength, HmacSize))
                    throw new CryptographicException("Decryption failed.");
            }

            using (Aes aes = CreateAes(encKey, iv))
            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            {
                byte[] plainBytes = decryptor.TransformFinalBlock(data, SaltSize + IvSize, cipherLength);
                try
                {
                    return Encoding.UTF8.GetString(plainBytes);
                }
                finally
                {
                    Array.Clear(plainBytes, 0, plainBytes.Length);
                }
            }
        }
        finally
        {
            Array.Clear(encKey, 0, encKey.Length);
            Array.Clear(macKey, 0, macKey.Length);
        }
    }

    private static void DeriveKeys(string password, byte[] salt, out byte[] encKey, out byte[] macKey)
    {
        byte[] keyMaterial;
        using (var kdf = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
        {
            keyMaterial = kdf.GetBytes(KeySize * 2); // 32 bytes for AES, 32 bytes for HMAC
        }

        encKey = new byte[KeySize];
        macKey = new byte[KeySize];
        Buffer.BlockCopy(keyMaterial, 0, encKey, 0, KeySize);
        Buffer.BlockCopy(keyMaterial, KeySize, macKey, 0, KeySize);
        Array.Clear(keyMaterial, 0, keyMaterial.Length);
    }

    private static Aes CreateAes(byte[] key, byte[] iv)
    {
        Aes aes = Aes.Create();
        aes.KeySize = KeySize * 8;
        aes.BlockSize = IvSize * 8;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = key;
        aes.IV = iv;
        return aes;
    }

    private static bool ConstantTimeEquals(byte[] a, int aOffset, byte[] b, int bOffset, int length)
    {
        int diff = 0;
        for (int i = 0; i < length; i++)
            diff |= a[aOffset + i] ^ b[bOffset + i];
        return diff == 0;
    }
}