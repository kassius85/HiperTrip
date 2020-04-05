namespace HiperTrip.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Determines if the object is null.
        /// </summary>
        /// <param name="objeto"></param>
        /// <returns>
        /// True if the object is null. False otherwise.
        /// </returns>
        public static bool IsNull(this object objeto)
        {
            return (objeto == null);
        }
    }
}