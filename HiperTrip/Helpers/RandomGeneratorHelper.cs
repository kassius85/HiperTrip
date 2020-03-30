using System;
using System.Text;

namespace HiperTrip.Helpers
{
    public static class RandomGeneratorHelper
    {
        // Generate a random number between two numbers    
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string number with a given size    
        public static string RandomString(int size)
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
    }
}