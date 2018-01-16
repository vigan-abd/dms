using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Domain_Model;
using Helpers.Security;
using Repository.UnitOfWork;
using System.Security.Cryptography;
using System.IO;
using Helpers.Algorithm;
using Service;

namespace TestingConsole
{
    class TestingConsole
    {
        static void Main(string[] args)
        {
            string[] files = new string[]
            {
                "Sunny beach picture",
                "PHP and MySQL Web Development",
                "PHP Development",
                "Testing2",
                "ASP Web Development"
            };
            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine(UserStorageService.GenerateExternalLink(files[i]));
            }
            Console.ReadKey();
        }
    }
}
