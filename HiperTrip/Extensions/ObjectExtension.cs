using System;

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

        //public static bool IsEmpty(this object objeto)
        //{
        //    switch (Type.GetTypeCode(objeto.GetType().GetProperty("objeto").PropertyType))
        //    {
        //        case TypeCode.Int32:
        //            {
        //                return 

        //                break;
        //            }
        //    }
        //}
    }
}