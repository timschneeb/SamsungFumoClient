using System;

namespace SamsungFumoClient.Utils
{
    public static class ArrayUtils
    {
        public static T[] ArrayPush<T>(this T[] table, object value)
        {
            Array.Resize(ref table, table.Length + 1); // Resizing the array for the cloned length (+-) (+1)
            table.SetValue(value, table.Length - 1); // Setting the value for the new element
            return table;
        }

        public static T[] ConcatArray<T>(params T[][] arrays)
        {
            int l, i;

            for (l = i = 0; i < arrays.Length; l += arrays[i].Length, i++) ;
            var a = new T[l];

            for (l = i = 0; i < arrays.Length; l += arrays[i].Length, i++)
                arrays[i].CopyTo(a, l);

            return a;
        }
    }
}