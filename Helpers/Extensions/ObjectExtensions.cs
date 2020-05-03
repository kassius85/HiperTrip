using System;
using System.Reflection;

namespace Helpers.Extensions
{
    public static class ObjectExtensions
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

        /// <summary>
        /// Maps an object type to another object type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objeto"></param>
        /// <returns></returns>
        public static T Map<T>(this object objeto) where T : new() 
        {
            int cantProp = 0;
            T result = new T();

            foreach (PropertyInfo infoPropiedad in result.GetType().GetProperties())
            {
                MethodInfo setMethod = infoPropiedad.GetSetMethod(false); // No devolver si el descriptor de acceso es no público.

                if (!setMethod.IsNull()) // Verificar que la propiedad tenga método set público.
                {
                    PropertyInfo property = objeto.GetType().GetProperty(infoPropiedad.Name);

                    if (!property.IsNull() && infoPropiedad.PropertyType == property.PropertyType)
                    {
                        infoPropiedad.SetValue(result, property.GetValue(objeto, null));
                        cantProp++;
                    }
                }                
            }

            if (cantProp.IsEmpty())
            {
                result = default;
            }

            return result;
        }
    }
}