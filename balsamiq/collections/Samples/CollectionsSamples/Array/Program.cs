using Array;

Listing1();

Listing2();

void Listing1()
{
    Console.WriteLine("Listing 1.");
    int[] numbers = new int[5];

    numbers[1] = 10;

    for (int i = 0; i < numbers.Length; i++)
    {
        Console.WriteLine($"Index: {i} - Value: {numbers[i]}");
    }

    Console.WriteLine();
}

void Listing2()
{
    Console.WriteLine("Listing 2.");

    int[] numbers = new int[3] { 1, 2, 3 };

    PrintArray.Print(numbers);
    Console.WriteLine($"Size: {numbers.Length}");

    int[] numbers2 = new int[] { 1, 2, 3, 4 };

    PrintArray.Print(numbers2);
    Console.WriteLine($"Size: {numbers2.Length}");

    // Błąd kompilacji
    //int[] numbers3 = new int[3] { 1, 2 };
}

void Listing3()
{

}