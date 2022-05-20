using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class NameGenerator
    {
        public static string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";


        public static string GenerateUsername(int length)
        {
            Random random = new Random();
            char[] s = new char[length];

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                s[i] = chars[index];
            }

            string res = new string(s) + "@test.com";
            return res;
        }


        public static string GenerateEntityName(int length)
        {
            Random random = new Random();
            char[] s = new char[length];

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                s[i] = chars[index];
            }

            string res = new string(s);
            return res;
        }


    }
}
