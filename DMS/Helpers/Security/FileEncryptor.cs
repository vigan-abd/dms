using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Security
{
    public static class FileEncryptor
    {
        private static byte[] Key
        {
            get
            {
                return Encoding.UTF8.GetBytes(
              ConfigurationManager.AppSettings.Get("CipherKey"));
            }
        }

        private static byte[] IV
        {
            get
            {
                return Encoding.UTF8.GetBytes(
              ConfigurationManager.AppSettings.Get("CipherIV"));
            }
        }

        private static Rijndael InitializeCipher()
        {
            Rijndael cipher = Rijndael.Create();
            cipher.Key = Key;
            cipher.Padding = PaddingMode.Zeros;
            cipher.Mode = CipherMode.CBC;
            cipher.IV = IV;
            return cipher;
        }

        public static void EncryptFile(string filename, Stream data, int length)
        {
            byte[] plainData;
            using (BinaryReader reader = new BinaryReader(data))
            {
                plainData = reader.ReadBytes(length);
            }
            EncryptFile(filename, plainData);
        }

        public static void EncryptFile(string filename, byte[] data)
        {
            Rijndael cipher = InitializeCipher();

            FileStream fsopen = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            CryptoStream cs = new CryptoStream(fsopen, cipher.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.Close();
            fsopen.Close();
        }

        public static byte[] DecryptFile(string filename)
        {
            Rijndael cipher = InitializeCipher();

            FileStream fsread = new FileStream(filename, FileMode.Open, FileAccess.Read);
            CryptoStream cs = new CryptoStream(fsread, cipher.CreateDecryptor(), CryptoStreamMode.Read);
            byte[] decrypted = new byte[fsread.Length];
            cs.Read(decrypted, 0, decrypted.Length);
            cs.Close();
            fsread.Close();

            return decrypted;
        }
    }
}
