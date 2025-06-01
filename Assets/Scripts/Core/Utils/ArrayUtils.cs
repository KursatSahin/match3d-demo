using System;

namespace Match3dCore.Utils
{
    public static class ArrayExtensions
    {
        // Static Random instance to avoid issues with the same seed if called in rapid succession
        // compared to creating a new Random() instance on each call.
        private static Random _random = new Random();

        /// <summary>
        /// Returns a randomly selected subarray of an array.
        /// The subarray consists of contiguous elements from the original array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The source array.</param>
        /// <returns>A randomly selected subarray. Returns an empty array if the source array is null or empty.</returns>
        public static T[] GetRandomSubarray<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
            {
                return Array.Empty<T>(); // Or new T[0];
            }

            // Randomly select the starting index of the subarray.
            // Next(minValue, maxValue) generates a random integer that is greater than or equal to minValue 
            // and less than maxValue; that is, the range of return values includes minValue but not maxValue.
            int startIndex = _random.Next(0, array.Length);

            // Randomly select the ending index of the subarray.
            // It must be greater than or equal to startIndex and less than array.Length.
            // This ensures the subarray has a length of at least 1.
            int endIndex = _random.Next(startIndex, array.Length);

            // Calculate the length of the subarray.
            int length = endIndex - startIndex + 1;

            // Create the new subarray.
            T[] subarray = new T[length];

            // Copy the elements from the original array to the subarray.
            Array.Copy(array, startIndex, subarray, 0, length);

            return subarray;
        }

        /// <summary>
        /// Returns a subarray of an array based on user-provided minimum and maximum indices.
        /// The subarray consists of contiguous elements from the original array.
        /// Rules for min and max:
        /// - min cannot be greater than max.
        /// - min cannot be less than 1.
        /// - max cannot be greater than the array's length.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The source array.</param>
        /// <param name="minIndex">The 1-based starting index for the subarray (inclusive).</param>
        /// <param name="maxIndex">The 1-based ending index for the subarray (inclusive).</param>
        /// <returns>A subarray based on the provided min and max indices. Returns an empty array if inputs are invalid.</returns>
        public static T[] GetRandomSubarrayWithMinMax<T>(this T[] array, int minIndex, int maxIndex)
        {
            if (array == null || array.Length == 0)
            {
                Console.WriteLine("Hata: Kaynak dizi boş veya null.");
                return Array.Empty<T>();
            }

            int arrayLength = array.Length;

            // Rule 1: min cannot be greater than max.
            if (minIndex > maxIndex)
            {
                Console.WriteLine("Hata: Minimum değer, maksimum değerden büyük olamaz.");
                return Array.Empty<T>();
            }

            // Rule 2: min cannot be less than 1.
            if (minIndex < 1)
            {
                Console.WriteLine("Hata: Minimum değer 1'den küçük olamaz.");
                return Array.Empty<T>();
            }

            // Rule 3: max cannot be greater than the array's length.
            if (maxIndex > arrayLength)
            {
                Console.WriteLine($"Hata: Maksimum değer, dizinin uzunluğundan ({arrayLength}) büyük olamaz.");
                return Array.Empty<T>();
            }

            // Convert 1-based indices to 0-based indices for C# array indexing.
            int startIndex = minIndex - 1;
            int endIndex = maxIndex - 1; // Inclusive ending index in 0-based

            int length = endIndex - startIndex + 1;

            T[] subarray = new T[length];
            Array.Copy(array, startIndex, subarray, 0, length);

            return subarray;
        }
    }
}