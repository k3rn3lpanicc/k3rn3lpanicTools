using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace k3rn3lpanicTools
{
    public class Encryption
    {
        public class FileEnc
        {
            static byte[] salt = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            /// <summary>
            /// You Can change the salt of encrytping files with this method
            /// </summary>
            /// <param name="_salt">The Salt Value to be set</param>
            public static void setSalt(byte[] _salt)
            {
                salt = _salt;
            }
            /// <summary>
            /// Gives you the salt used in encrpting/decrypting files
            /// </summary>
            /// <returns>salt</returns>
            public static byte[] getSalt()
            {
                return salt;
            }

            /// <summary>
            /// Nod Done yet
            /// </summary>
            /// <param name="Filename"></param>
            /// <param name="Password"></param>
            public static void Fast_Encrypt(string Filename,string Password)
            {

            }

            public static void AESFAST_enCrypt(string filePath, string passWord)
            {
                //File.SetAttributes(filePath, FileAttributes.Normal);

                byte[] filestobeEncrypted = File.ReadAllBytes(filePath);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(passWord);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesEncrypted = _Encrypt(filestobeEncrypted, passwordBytes);
                File.WriteAllBytes(filePath, bytesEncrypted);
            }

            public static void AESFAST_DeCrypt(string filePath, string passWord)
            {
                //File.SetAttributes(filePath, FileAttributes.Normal);

                byte[] filestobeDecrypted = File.ReadAllBytes(filePath);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(passWord);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesDecrypted = _Decrypt(filestobeDecrypted, passwordBytes);
                File.WriteAllBytes(filePath, bytesDecrypted);
            }

            public static byte[] _Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
            {
                byte[] encryptedBytes = null;

                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                using (MemoryStream ms = new MemoryStream())
                {
                    using (RijndaelManaged AES = new RijndaelManaged())
                    {
                        AES.KeySize = 256;
                        AES.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        AES.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                            cs.Close();
                        }
                        encryptedBytes = ms.ToArray();
                    }
                }

                return encryptedBytes;
            }

            public static byte[] _Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
            {
                byte[] decryptedBytes = null;

                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                using (MemoryStream ms = new MemoryStream())
                {
                    using (RijndaelManaged AES = new RijndaelManaged())
                    {
                        AES.KeySize = 256;
                        AES.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        AES.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                            cs.Close();
                        }
                        decryptedBytes = ms.ToArray();
                    }
                }

                return decryptedBytes;
            }
            /// <summary>[AES]Encrypts a file using a password , usage : EncryptFile(mother[mm], mother[mm].Split('.')[0] + "_1" + ".gM", "☺◘♠ef!$U@#R", new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 1024);</summary>
            /// <param name="sourceFilename">The full path and name of the file to be encrypted.</param>
            /// <param name="destinationFilename">The full path and name of the file to be output.</param>
            /// <param name="password">The password for the encryption.</param>
            /// <param name="iterations">The number of iterations Rfc2898DeriveBytes should use before generating the key and initialization vector for the decryption.</param>
            public static void AES_EncryptFile(string sourceFilename, string destinationFilename, string password, int iterations)
            {
                AesManaged aes = new AesManaged();
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                // NB: Rfc2898DeriveBytes initialization and subsequent calls to   GetBytes   must be eactly the same, including order, on both the encryption and decryption sides.
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, iterations);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                aes.Mode = CipherMode.CBC;
                ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);

                using (FileStream destination = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
                    {
                        using (FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            source.CopyTo(cryptoStream);
                        }
                    }
                }
            }
            /// <summary>[AED]Decrypt Files using a password</summary>
            /// <remarks>NB: "Padding is invalid and cannot be removed." is the Universal CryptoServices error.  Make sure the password, salt and iterations are correct before getting nervous.</remarks>
            /// <param name="sourceFilename">The full path and name of the file to be decrypted.</param>
            /// <param name="destinationFilename">The full path and name of the file to be output.</param>
            /// <param name="password">The password for the decryption.</param>
            /// <param name="iterations">The number of iterations Rfc2898DeriveBytes should use before generating the key and initialization vector for the decryption.</param>
            public static void AES_DecryptFile(string sourceFilename, string destinationFilename, string password, int iterations)
            {
                AesManaged aes = new AesManaged();
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                // NB: Rfc2898DeriveBytes initialization and subsequent calls to   GetBytes   must be eactly the same, including order, on both the encryption and decryption sides.
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(password, salt, iterations);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);
                aes.Mode = CipherMode.CBC;
                ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);

                using (FileStream destination = new FileStream(destinationFilename, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(destination, transform, CryptoStreamMode.Write))
                    {
                        try
                        {
                            using (FileStream source = new FileStream(sourceFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                source.CopyTo(cryptoStream);
                            }
                        }
                        catch (CryptographicException exception)
                        {
                            if (exception.Message == "Padding is invalid and cannot be removed.")
                                throw new ApplicationException("Universal Microsoft Cryptographic Exception (Not to be believed!)", exception);
                            else
                                throw;
                        }
                    }
                }
            }
        }
        public class StringEnc
        {
            /// <summary>
            /// K3rn3lPanic 2021
            /// AES Encryption for strings
            /// </summary>
            /// <param name="clearText">The Text u want to encrypt.</param>
            /// <param name="password">Set the password for encryption</param>
            /// <returns>Encrypted Text</returns>
            public static string AES_Encryptstr(string clearText, string password)
            {
                string EncryptionKey = password;
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }
            /// <summary>
            /// K3rn3lPanic 2021
            /// AES Decryption method using a password
            /// </summary>
            /// <param name="cipherText">The Encrypted text u wish to decrypt</param>
            /// <param name="password">The password to be used in Decryption</param>
            /// <returns>Decrypted Text</returns>
            public static string AES_Decryptstr(string cipherText, string password)
            {
                string EncryptionKey = password;
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }
        }
        public class Hashing {
            static private string GetMd5Hash(MD5 md5Hash, string input)
            {
                
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
            public static string getMD5(string data)
            {
                string hash = "";
                using (MD5 md5Hash = MD5.Create())
                {
                   hash = GetMd5Hash(md5Hash, data);
                }
                return hash;
            }
        }
    }
}
