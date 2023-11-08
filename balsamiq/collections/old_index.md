# Tablica - `Array<T>`

Klasa `Array<T>` to podstawa wszystkich jedno- i wielowymiarowych tablic oraz jeden z podstawowych typów implementujących standardowe interfejsy kolekcji.

Ze względu na szczególne znaczenie tablic, które są podstawą większości przeliczalnych kolekcji projektanci języka stworzyli specjalną składnie do ich deklarowania oraz inicjalizacji - `T[] tab = new T[]`, gdzie `T` jest dowolnym typem:

_Listing 1._

```csharp
int[] numbers = new int[5]; // tworzy tablicę 5 elementów, z których każdy element ma wartość 0
numbers[1] = 10; // tablica ma postać - 0 10 0 0 0
```

Tablice możemy również inicjalizować wartościami bezpośrednio w konstruktorze, dzięki temu możemy również prznieść dedukcję rozmiaru tablicy na kompilator:

_Listing 2._

```csharp
int[] numbers = new int[3] { 1, 2, 3 };

int[] numbers2 = new int[] { 1, 2, 3, 4 }; // kompilator wydedukuje rozmiar tablicy 4

int[] numbers3 = new int[3] { 1, 2 }; // błąd kompilacji - rozmiar nie zgadza się z zawartością
```

Tablice mogą przechowywać dowolny typy, czy to wartościowy czy referencyjny. W przypadku typów wartościowych są one przechowywane bezpośrednio w tablicy, dla przykładu weźmy tablicę 5 liczb całkowitych (każda zajmuje 8 bajtów), więc system CLR zarezerwuje ciągły obszar pamięci o wielkości 40 bajtów. W przypadku typów referencyjnych (które ważą 4 bajty w środowisku 32-bitowym i 8 bajtów w 64-bitowym) system CLR dla tablicy o rozmiarze 3 zarezerwuje 24 bajty (12 w środowisku 32-bitowym).

**Uwaga:** pamiętaj, że tablice są typami referencyjnymi, niezależnie od rodzaju przechowywanych w niej elementów i podlegają wszystkim zasadom typów referencyjnych, dlatego też wynikiem działania kodu `array2 = array1` będą dwie zmienne odnoszące się do tej samej tablicy. Przy czym porównanie dwóch tablic zawsze da wynik negatywny (chyba, że programista użyje własnego komparatora).

Tablice jako typ przeliczalny odpowiadają standardowym operacjom przeglądania przy pomocy pętli.

## Rozmiar Tablicy

Cechą charakterystyczną tablic jest ich niezmienny rozmiar. Zauważ, że tworząc tablicę zawsze musisz przekazać jej rozmiar. Dzięki takiemu założeniu system CLR przypisze utworzonej tablicy ciągły obszar w pamięci. Dzięki temu indeksowanie jest bardzo szybkie, ale za cenę braku możliwości rozszerzenia rozmiaru w późniejszym czasie (w prosty, bezkosztowy sposób). Do odczytania rozmiaru tablicy służy właściwość `Length`.

Gdyby ktoś Cię kiedyś spytał czy można zmienić rozmiar tablicy to odpowiedź brzmi - tak. Można w tym celu wywołać metodę `Resize`, przy czym jej działanie polega na utworzeniu nowej większej tablicy i skopiowaniu do niej wszystkich elementów. Nie dość, że operacja jest nieefektywna to dodatkowo wszystkie referencje nadal będą wskazywać na starą strukturę. W takim przypadku zalecane jest użycie listy.

## Porównanie tablic

W obecnej wersji .NET istnieje już wbudowany komparator, który pozwala na porównanie dwóch tablic na podstawie ich zawartości. Zostało to przedstawione na poniższym przykładzie:

_Listing 3._

```csharp
int[] array1 = new int[] { 1, 2, 3 };

int[] array2 = new int[] { 1, 2, 3 };

bool operatorEqual = array1 == array2;

bool linqEqual = array1.SequenceEqual(array2); // Uwaga: Kolejność elementów w tablicy ma znaczenie

IStructuralEquatable se = array1;
bool structuralEqual = se.Equals(array2, StructuralComparisons.StructuralEqualityComparer);

Console.WriteLine(operatorEqual); // False
Console.WriteLine(linqEqual); // True
Console.WriteLine(structuralEqual); // True
```

## Przeszukiwanie tablic

Klasa Array udostępnia wiele metod do znajdowania elementów w tablicach jednowymiarowych.

- `BinarySearch` - przeszukiwanie posortowanej tablicy
- `IndexOf`, `LastIndex` - indeks szukanego elementu (pierwszy lub ostatni napotkany) w nieposortownej tablicy oparte o metodę `Equals`.
- `Find*`, `Exists` - metody do przeszukiwania nieposortowanych tablic spełniających warunek.

_Listing 4._

```csharp
char[] signs = new char[] { 'a', 'b', 'c', 'd', 'e' };
int index = signs.IndexOf('c') // 2
```

## Sortowanie tablic

Tablica zapewnia możliwość sortowania elementów przy użyciu metody `Array.Sort()`. Metoda ta wymaga, aby znajdujące sie w tablicy elementy implementowały interfejs `IComparable`.

_Listing 5._

```csharp
char[] signs = new char[] { 'c', 'e', 'd', 'b', 'a' };
Array.Sort(signs) // a b c d e
```

## Tablice wielowymiarowe

Kolejną zaletą tablic jest wsparcie dla kolekcji wielowymiarowej. Możemy utworzyć np. tablicę dwuwymiarową, która mogłaby odpowiadać dwywymiarowej planszy. Dla przykładu spróbujmy utworzyć planszę do gry w szachy i na polu 3D umieśćmy królową, która odpowiada literze Q.

_Listing 6._

```csharp
char[,] chessBoard = new char[8,8]; // pusta tablica dwuwymiarowa 8x8

chessBoard[2, 3] = 'Q'; // umieszczamy w 3 wierszu i 4 kolumnie znak 'Q'
```

## Podsumowanie

Podsumowując tablica jest jednym z podstawowych typów kolekcji w języku C#. Jej cechą charakterystyczną jest stały rozmiar, który w znacznym stopniu przyspiesza przeglądanie takiej tablicy. Dodatkową zaletą tablicy jest możliwość tworzenia tablic wielowymiarowych imitujących jakieś powierzchnie.

Do częstych zastosowań tablic po za standardowymi, gdzie chcemy przechowywać zbiór obiektów i mamy pewność, że w przyszłości ich liczba się nie zmieni są również dane wielowymiarowe, które mają stałą wielkość np. obrazy, dźwięki, filmy, wszelkiego rodzaju modele matematyczne. Kolejnym przykładem użycia tablic może być ich zastosowanie w przenoszeniu wiadomości, które są odczytywane jako tablice bajtów. Wiemy, że tak odczytana wiadomość z pewnością nie będzie modyfikowana, a jedynie odczytywana w celu dalszego procesowania.

**Linki:**

- [MSDN - Array (C#)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/)
- [Ciekawy Ebook o kolekcjach (strony 6-21)](https://dev-hobby.pl/wordpress/wp-content/uploads/wpcfto_files/c91d5f5cff94bdb8475108b7dae84e52book.pdf)
