using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qoollo.MpegDash
{
    internal static class ArrayExtensions
    {
        /// <summary>
        /// Checks whether first array starts with second array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool StartsWith<T>(this T[] first, T[] second)
            where T: IComparable<T>
        {
            bool res = true;
            for (int i = 0; i < second.Length && res; i++)
            {
                res = first[i].CompareTo(second[i]) == 0;
            }
            return res;
        }
    }
}
