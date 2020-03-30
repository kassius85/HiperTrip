using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiperTrip.Extensions
{
    public static class ObjectExtension
    {
        public static bool IsNull(this object objeto)
        {
            return (objeto == null);
        }
    }
}