using System;
using System.Text;

namespace Helpers.Extensions
{
    public static class IntRandomExtensions
    {
        // Generate a random string number with a given size    
        public static string RandomString(this int size)
        {
            StringBuilder builder = new StringBuilder();
            string digit;

            for (int i = 0; i < size; i++)
            {
                digit = Convert.ToInt32(RandomNumber(0, 9)).ToString();
                builder.Append(digit);
            }

            return builder.ToString();
        }

        // Generate a random number between two numbers    
        private static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
    }
}