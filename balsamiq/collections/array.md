# Tablica - `Array<T>`

Klasa `Array<T>` to podstawa wszystkich jedno- i wielowymiarowych tablic oraz jeden z podstawowych typów implementujących standardowe interfejsy kolekcji.

Ze względu na szczególne znaczenie tablic, które są podstawą większości przeliczalnych kolekcji projektanci języka stworzyli specjalną składnie do ich deklarowania oraz inicjalizacji - `T[] tab = new T[]`, gdzie `T` jest dowolnym typem:

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

Cechą charakterystyczną tablic jest ich niezmienny rozmiar. Zauważ, że tworząc tablicę zawsze musisz przekazać jej rozmiar. Dzięki takiemu założeniu system CLR przypisze utworzonej tablicy ciągły obszar w pamięci. Dzięki temu indeksowanie jest bardzo szybkie, ale za cenę braku możliwości rozszerzenia rozmiaru w późniejszym czasie (w prosty, bezkosztowy sposób). Do odczytania rozmiaru tablicy służy właściwość `Length`.

Podsumowując tablica jest jednym z podstawowych typów kolekcji w języku C#. Jej cechą charakterystyczną jest stały rozmiar, który w znacznym stopniu przyspiesza przeglądanie takiej tablicy. Dodatkową zaletą tablicy jest możliwość tworzenia tablic wielowymiarowych imitujących jakieś powierzchnie.

Do częstych zastosowań tablic po za standardowymi, gdzie chcemy przechowywać zbiór obiektów i mamy pewność, że w przyszłości ich liczba się nie zmieni są również dane wielowymiarowe, które mają stałą wielkość np. obrazy, dźwięki, filmy, wszelkiego rodzaju modele matematyczne. Kolejnym przykładem użycia tablic może być ich zastosowanie w przenoszeniu wiadomości, które są odczytywane jako tablice bajtów. Wiemy, że tak odczytana wiadomość z pewnością nie będzie modyfikowana, a jedynie odczytywana w celu dalszego procesowania.

**Dodatkowe informacje:**

- Tablice mogą przechowywać dowolny typy, czy to wartościowy czy referencyjny. W przypadku typów wartościowych są one przechowywane bezpośrednio w tablicy, dla przykładu weźmy tablicę 5 liczb całkowitych (każda zajmuje 8 bajtów), więc system CLR zarezerwuje ciągły obszar pamięci o wielkości 40 bajtów. W przypadku typów referencyjnych (które ważą 4 bajty w środowisku 32-bitowym i 8 bajtów w 64-bitowym) system CLR dla tablicy o rozmiarze 3 zarezerwuje 24 bajty (12 w środowisku 32-bitowym).

- Gdyby ktoś Cię kiedyś spytał czy można zmienić rozmiar tablicy to odpowiedź brzmi - tak. Można w tym celu wywołać metodę `Resize`, przy czym jej działanie polega na utworzeniu nowej większej tablicy i skopiowaniu do niej wszystkich elementów. Nie dość, że operacja jest nieefektywna to dodatkowo wszystkie referencje nadal będą wskazywać na starą strukturę. W takim przypadku zalecane jest użycie listy.

- pamiętaj, że tablice są typami referencyjnymi, niezależnie od rodzaju przechowywanych w niej elementów i podlegają wszystkim zasadom typów referencyjnych, dlatego też wynikiem działania kodu `array2 = array1` będą dwie zmienne odnoszące się do tej samej tablicy. Przy czym porównanie dwóch tablic zawsze da wynik negatywny (chyba, że programista użyje własnego komparatora).

**Linki:**

- [MSDN - Array (C#)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/)
- [Ciekawy Ebook o kolekcjach (strony 6-21)](https://dev-hobby.pl/wordpress/wp-content/uploads/wpcfto_files/c91d5f5cff94bdb8475108b7dae84e52book.pdf)
