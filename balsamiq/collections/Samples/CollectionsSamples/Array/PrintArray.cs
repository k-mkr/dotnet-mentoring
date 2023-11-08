namespace Array
{
    internal static class PrintArray
    {
        public static void Print<T>(T[] array)
        {
            foreach (var item in array)
            {
                Console.Write($"{item}\t");
            }
            Console.WriteLine();
        }
    }
}
