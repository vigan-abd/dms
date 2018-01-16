using System;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Helpers.Security
{
    public static class PasswordHelper
    {
        public static string ComputeHash(string input)
        {
            SHA1 hashAlg = SHA1.Create();
            byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hash).Replace("-", "");
        }

        public static string ComputePassword(string password, string salt)
        {
            return ComputeHash(password + salt);
        }

        public static string GenerateSalt()
        {
            string salt = new Random(DateTime.Now.Millisecond).Next().ToString();
            if (salt.Length < 6)
            {
                int diff = 6 - salt.Length;
                for (int i = 0; i < diff; i++)
                {
                    salt += '3';
                }
            }
            else
            {
                salt = salt.Substring(0, 6);
            }
            return salt;
        }

        public static void ValidatePassword(string pwd)
        {
            string pattern = @"^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{8,}$";
            if(!Regex.IsMatch(pwd, pattern))
            {
                throw new Exception("Password must be at least 8 chars long and must contain at least one upper char and digit");
            }
        } 
    }
}
