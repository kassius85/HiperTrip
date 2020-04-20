namespace HiperTrip.Extensions
{
    public static class StringGenericExtension
    {
        /// <summary>
        /// Determines if the value is equivalent to true.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsStringTrue(this string value)
        {
            return (value == "S");
        }

        public static bool IsStringSuccess(this string value)
        {
            return (value.ToLower() == "success");
        }
    }
}