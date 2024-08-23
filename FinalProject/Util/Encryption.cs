using System.Security.Cryptography;
using System.Text;

public static class Encryption
{
    public static string AES_Encrypt_ECB_128(string plainText, string EncryptKey)
    {
        if (String.IsNullOrEmpty(plainText) || String.IsNullOrEmpty(EncryptKey))
            return "";
        string cipherText = "";
        try
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = mkey(EncryptKey); //Encoding.UTF8.GetBytes(Iat);
                aesAlg.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                byte[] clearBytes = Encoding.UTF8.GetBytes(plainText);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(clearBytes, 0, clearBytes.Length);
                    }
                    cipherText = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            return cipherText;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return cipherText;
    }

    public static string AES_Decrypt_ECB_128(string cipherText, string EncryptKey)
    {
        if (cipherText == "")
            return "";

        string plainText = "";
        try
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 128;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Key = mkey(EncryptKey);
                aesAlg.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plainText;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return plainText;
    }

    public static string Encrypt_CBC_256(string PlainText, string EncryptionKey, string SaltKey)
    {

        string encryptString = "";
        try
        {
            var bsaltkey = Encoding.UTF8.GetBytes(SaltKey);
            byte[] clearBytes = Encoding.UTF8.GetBytes(PlainText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, bsaltkey, 1000, HashAlgorithmName.SHA256);
                encryptor.Key = pdb.GetBytes(32);  //256 bit Key
                encryptor.IV = GenerateRandomBytes(16);
                encryptor.Mode = CipherMode.CBC;
                encryptor.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                    }
                    byte[] result = MergeArrays(encryptor.IV, ms.ToArray());  //append IV to cipher, so cipher length will longer
                    encryptString = Convert.ToBase64String(result);

                }
            }
            return encryptString;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return encryptString;
    }

    public static string Decrypt_CBC_256(string cipherText, string EncryptionKey, string SaltKey)
    {
        string plainText = "";
        try
        {
            var bsaltkey = Encoding.UTF8.GetBytes(SaltKey);

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, bsaltkey, 1000, HashAlgorithmName.SHA256);
                encryptor.Mode = CipherMode.CBC;
                encryptor.Padding = PaddingMode.PKCS7;
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = cipherBytes.Take(16).ToArray();
                cipherBytes = cipherBytes.Skip(16).ToArray();

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                    }
                    plainText = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            return plainText;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return plainText;
    }

    private static byte[] MergeArrays(params byte[][] arrays)
    {
        var merged = new byte[arrays.Sum(a => a.Length)];
        var mergeIndex = 0;
        for (int i = 0; i < arrays.GetLength(0); i++)
        {
            arrays[i].CopyTo(merged, mergeIndex);
            mergeIndex += arrays[i].Length;
        }

        return merged;
    }
    private static byte[] GenerateRandomBytes(int numberOfBytes)
    {
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        var randomBytes = new byte[numberOfBytes];
        rng.GetBytes(randomBytes);
        return randomBytes;
    }
    private static byte[] mkey(string skey)
    {

        byte[] key = Encoding.UTF8.GetBytes(skey);
        byte[] k = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < key.Length; i++)
        {
            k[i % 16] = (byte)(k[i % 16] ^ key[i]);
        }

        return k;
    }
}
