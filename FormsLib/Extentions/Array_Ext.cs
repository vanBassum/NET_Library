using System;

namespace FormsLib.Extentions
{
    public static partial class Array_Ext
    {
        /// <summary>
        /// Creates a copy of this array.
        /// </summary>
        /// <typeparam name="T">Type of items in array</typeparam>
        /// <param name="data">The array to copy</param>
        /// <param name="index">Start index</param>
        /// <param name="length">Number of items to be copied</param>
        /// <returns>A partial copy of the origional</returns>
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// Creates a copy of this array.
        /// </summary>
        /// <typeparam name="T">Type of items in array</typeparam>
        /// <param name="data">The array to copy</param>
        /// <param name="index">Start index</param>
        /// <returns>A partial copy of the origional</returns>
        public static T[] SubArray<T>(this T[] data, int index)
        {
            T[] result = new T[data.Length - index];
            Array.Copy(data, index, result, 0, data.Length - index);
            return result;
        }
    }



}
