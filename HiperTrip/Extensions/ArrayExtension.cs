using System.Linq;

namespace HiperTrip.Extensions
{
    public static class ArrayExtension
    {
        /// <summary>
        /// Verifica que dos arreglos sean iguales.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arreglo1"></param>
        /// <param name="arreglo2"></param>
        /// <returns></returns>
        public static bool ArraysEquals<T>(this T[] arreglo1, T[] arreglo2)
        {
            // Ambos arreglos son nulos: true
            if (arreglo1 == null && arreglo2 == null)
            {
                return true;
            }

            // Uno de los dos es nulo: false
            if (arreglo1 == null || arreglo2 == null)
            {
                return false;
            }

            // No tienen el mismo tamaño (cantidad de elementos): false
            if (arreglo1.Length != arreglo2.Length)
            {
                return false;
            }

            // Compararlos elemento por elemento.
            return arreglo1.SequenceEqual(arreglo2);
        }
    }
}