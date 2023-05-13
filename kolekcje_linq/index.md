# Kolekcje i wywołania LINQ

Witak w kolejnym module! Tym razem zajmiemy się kolekcjami w języku C# oraz operatorami LINQ. W poprzednich modułach miałeś już okazję korzystać z tablic (`new T[]`) oraz list (`new List<T>()`). Tym razem zajmiemy się tematem trochę głębiej poznając temat od strony teoretycznej jak i praktycznej. Na koniec tego modułu chciałbym żebyś umiał efektywnie przechowywać i zarządzać zbiorami obiektów oraz wiedział w jaki sposób dobierać typ kolekcji do wymagań oraz czym się charakteryzuje i jakie ma wady oraz zalety każda z nich.

Poznasz również LINQ (Language-Integrated Query), który ułatwia obsługę kolekcji poprzez możliwości sortowania, filtrowania w sposób prosty i szybki bez używania pętli - przy czym szybkość będzie tutaj miała pojęcie względne o czym przekonasz się w dalszej części.

## 1. Co to są kolekcje i dlaczego są ważne?

Platforma .NET zapewnia standardowy zestaw typów kolekcji, które pozwalają na obsługę dowolnych zbiorów danych. Wśród nich możemy znaleźć listy o zmiennym rozmiarze (np. listy), listy o stałym rozmiarze, słowniki, czy tablice. Typów jest cała masa i tylko od naszych wymagań zależy, który z nich powinien zostać użyty.

Ważne żebyś zapamiętał, że spośród wszystkich kolekcji tylko tablice są wbudowane w język C#. Reszta typów kolekcji to wbudowane klasy, które w jakiś sposób rozszerzają tablice dodając im dodatkowych funkcjonalności.

Typy kolekcji możemy podzielić na następujące kategorie:

- Interfejsy definiujące standardowe protokoły kolekcji np. `IReadOnlyList<T>`;
- Gotowe do użycia klasy kolekcji np. `List<T>`, `Dictionary<TKey, TValue>`;
- Klasy bazowe do pisania kolekcji specjalnie dostosowywanych do potrzeb konkretnych aplikacji np. `CollectionBase`.

**Uwaga:** Zauważ, że większość kolekcji przedstawiana jest jako typy generyczne, wynika to z tego, że chcemy żeby mogły pracować z dowolnymi typami podanymi przez programistę.

Głównym czynnikiem przemawiającym za wykorzystywaniem kolekcji jest ich skuteczność i wydajność. Zostały przez twórców zaprojektowane tak, aby były wydajne i efektywne. Pozwalają na przechowywanie i zarządzanie dużymi zbiorami danych. Dodatkowo dostarczają szereg metod, który pokrywają większość zapotrzebowania użytkownika (dodawanie, usuwanie itd.). Kolejnym ważnym czynnikiem jest bezpieczeństwo typów, które jak już pewnie zdążyłeś się przekonać jest podstawowym wymaganiem języka C#. Dzięki temu masz pewność, że obiekty przechowywane w kolekcji będą zgodne w zakresie typu.

## 2. Typy kolekcji, czyli co, jak, kiedy?

W tym podrozdziale zajmiemy się najpopularniejszymi kolekcjami występującymi w języku C#. Omówimy sobie ich definicję, przeanalizujemy najpopularniejsze metody oraz zastanowimy się jakie są ich zastosowania na podstawie rzeczywistych przykładów.

### 2.1 Tablice - `Array<T>`

Klasa `Array<T>` to podstawa wszystkich jedn- i wielowymiarowych tablić oraz jeden z podstawowych typów implementujących standardowe interfejsy kolekcji. Ponieważ tablica jest generyczna, dzięki temu wszystkie tablice dysponują takim samym zestawem metod niezależnie od ich deklaracji i typu elementów.

Mogłeś też zauważyć, że tablice deklarujemy inaczej niż standardowe klasy - `int[] tab = new int[]`. Zostało to zrobione ze względu na wielkie znaczenie tablic w języku C# i z tego powodu utworzona specjalną składnię do ich deklarowania i inicjalizowania.

Cechą charakterystyczną tablic jest ich niezmienny rozmiar. Zauważ, że tworząc tablicę zawsze musisz przekazać jej rozmiar. Dzięki takiemu założeniu system CLR przypisze utworzonej tablicy ciągły obszar w pamięci. Dzięki temu indeksowanie jest bardzo szybkie, ale za cenę braku możliwości rozszerzenia rozmiaru w późniejszym czasie.

Tablice mogą przechowywać dowolny typy, czy to wartościowy czy referencyjny. W przypadku typów wartościowych są one przechowywane bezpośrednio w tablicy, dla przykładu weźmy tablicę 5 liczb całkowitych (każda zajmuje 8 bajtów), więc system CLR zarezerwuje ciągły obszar pamięci o wielkości 40 bajtów. W przypadku typów referencyjnych (które ważą 4 bajty w środowisku 32-bitowym i 8 bajtów w 64-bitowym) system CLR dla tablicy o rozmiarze 3 zarezerwuje 24 bajty (12 w środowisku 32-bitowym).

**Uwaga:** pamiętaj, że tablice są typami referencyjnymi, niezależnie od rodzaju przechowywanych w niej elementów i podlegają wszystkim zasadom typów referencyjnych, dlatego też wynikiem działania kodu `array2 = array1` będą dwie zmienne odnoszące się do tej samej tablicy. Przy czym porównanie dwóch tablic zawsze da wynik negatywny (chyba, że programista użyje własnego komparatora).

W obecnej wersji .NET istnieje już wbudowany komparator, który pozwala na porównanie dwóch tablic na podstawie ich zawartości. Zostało to przedstawione na poniższym przykładzie:

```csharp
int[] array1 = new int[] { 1, 2, 3 };

int[] array2 = new int[] { 1, 2, 3 };

bool operatorEqual = array1 == array2;

bool linqEqual = array1.SequenceEqual(array2);

IStructuralEquatable se = array1;
bool structuralEqual = se.Equals(array2, StructuralComparisons.StructuralEqualityComparer);

Console.WriteLine(operatorEqual); // False
Console.WriteLine(linqEqual); // True
Console.WriteLine(structuralEqual); // True
```

Należy uważać, ponieważ przy porównaniu kolejność ma znaczenie, w przypadku tablicy `int[] array2 = new int[] { 3, 2, 1 }` wszystkie porównania zwróciłyby wartość `False`.

Gdyby ktoś Cię kiedyś spytał czy można zmienić rozmiar tablicy to odpowiedź brzmi - tak. Można w tym celu wywołać metodę `Resize`, przy czym jej działanie polega na utworzeniu nowej większej tablicy i skopiowaniu do niej wszystkich elementów. Nie dość, że operacja jest nieefektywna to dodatkowo wszystkie referencje nadal będą wskazywać na starą strukturę. W takim przypadku zalecane jest użycie listy.

Możemy się przyjrzeć bliżej inicjalizacji tablic. Najlepszą praktyką jest tworzenie tablicy przy pomocy konstruktora `new T[]`:

```csharp
int[] numbers = new int[5]; // tworzy tablicę 5 elementów, z których każdy element ma wartość 0
numbers[1] = 10; // tablica ma postać - 0 10 0 0 0
```

Tablice możemy również inicjalizować wartościami bezpośrednio w konstruktorze, dzięki temu możemy również prznieść dedukcję rozmiaru tablicy na kompilator:

```csharp
int[] numbers = new int[3] { 1, 2, 3 };

int[] numbers2 = new int[] { 1, 2, 3, 4 }; // kompilator wydedukuje rozmiar tablicy 4

int[] numbers3 = new int[3] { 1, 2 }; // błąd kompilacji - rozmiar nie zgadza się z zawartością
```

Tablice jako typ przeliczalny odpowiadają standardowym operacjom przeglądania przy pomocy pętli.

Kolejną zaletą tablic jest wsparcie dla kolekcji wielowymiarowej. Możemy utworzyć np. tablicę dwuwymiarową, która mogłaby odpowiadać dwywymiarowej planszy. Dla przykładu spróbujmy utworzyć planszę do gry w szachy i na polu 3D umieśćmy królową, która odpowiada literze Q.

```csharp
char[,] chessBoard = new char[8,8]; // pusta tablica dwuwymiarowa 8x8

chessBoard[2, 3] = 'Q'; // umieszczamy w 3 wierszu i 4 kolumnie znak 'Q'
```

Tak naprawdę ciężko wskazać wszystkie metody, które zostały zaimplementowane w standardzie i mogą być używane na tablicach. Wszystko zależy od zdefiniowanych wymagań i potrzeb w danej chwili. Najlepszym sposobem jest utworzenie sobie przykładowej tablicy i przejrzenie, które z nich są dostęne i jaką spełniają funkcję:

![Array Methods](./imgs/array_methods.png)

Podsumowując tablica jest jednym z podstawowych typów kolekcji w języku C#. Jej cechą charakterystyczną jest stały rozmiar, który w znacznym stopniu przyspiesza przeglądanie takiej tablicy. Dodatkową zaletą tablicy jest możliwość tworzenia tablic wielowymiarowych imitujących jakieś powierzchnie.

Do częstych zastosowań tablic po za standardowymi, gdzie chcemy przechowywać zbiór obiektów i mamy pewność, że w przyszłości ich liczba się nie zmieni są również dane wielowymiarowe, które mają stałą wielkość np. obrazy, dźwięki, filmy, wszelkiego rodzaju modele matematyczne. Kolejnym przykładem użycia tablic może być ich zastosowanie w przenoszeniu wiadomości, które są odczytywane jako tablice bajtów. Wiemy, że tak odczytana wiadomość z pewnością nie będzie modyfikowana, a jedynie odczytywana w celu dalszego procesowania.

## 2.2 Listy - `List<T>`

Po tablicach przyszedł czas na ich bardziej elastyczny odpowiednik. Generyczna klasa `List<T>` umożliwiają tworzenie tablic obiektów o dynamicznym rozmiarze i należą do najczęściej używanych klas kolekcji.

Zastanówmy się chwilę nad wewnętrzną implementacją klasy `List<T>`. Wewnętrznie klasa `List<T>` przechowuje tablicę elementów `T[]`, która najczęściej na początku ma rozmiar 4 (chyba, że programista zdecyduje inaczej). Następnie przy operacji dodawania `Add(T item)` kolejny element umieszczany jest w kolejnym wolnym indeksie tablicy. W przypadku gdy maksymalny rozmiar został osiągnięty wewnętrzna tablica zwiększa swój rozmiar (najczęściej dwukrotnie), a następnie poprzednia tablica kopiowana jest do nowoutworzonej i dodawany element umieszczany jest w pierwszym wolnym miejscu. Identyczna zasada działania jest dla operacji `Remove(T item)`. Jeśli wewnętrzna tablica jest zajęta w mniej niż połowie jest ona na identycznej zasadzie kopiowana do mniejszego egzemplarza.

Przyjrzyjmy się sposobom deklaracji listy. Są one dość podobne do tych użytych przy inicjalizacji tablic:

```csharp
List<int> numbers = new List<int>(); // Pusta lista
numbers.Add(10); // Zawartość listy - 10
numbers.Add(20); // Zawartość listy - 10 20

List<int> numbers2 = new List<int> { 10, 20 }; // Zawartość listy - 10 20

List<int> numbers3 = new List<int>(10); // Pusta lista z wewnętrzną tablicą o rozmiarze 10

int[] sourceArr = new int[] { 3, 2, 1 };
List<int> numbers4 = new List<int>(sourceArr); // Zawartość listy 3 2 1
numbers4.Add(5); // Zawartość listy 3 2 1 5
```

Każdy z konstruktorów ma swoje zastosowanie i domyślnego konstruktora

TODO:
Dodawania, a wstawianie elementów
