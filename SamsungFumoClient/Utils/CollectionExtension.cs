using System.Collections.Generic;

namespace SamsungFumoClient.Utils
{
    public static class CollectionExtension
    {
        public static T RandomElement<T>(this IList<T> list)
        {
            return list[RandomProvider.Random.Next(list.Count)];
        }

        public static T RandomElement<T>(this T[] array)
        {
            return array[RandomProvider.Random.Next(array.Length)];
        }
    }
}