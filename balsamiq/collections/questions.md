# Knowledge Check

**1. Jakie znasz typy kolekcji w C#, opisz ich przeznaczenie?**

- Lista (`List<T>`) - kolekcja o zmiennym rozmiarze, jest jedną z najbardziej uniwersalnych
- Tablica (`T[]`) - kolekcja o stałym rozmiarze, które deklarujemy podczas inicjalizacji
- Słownik (`Dictionary<TKey, TValue>`) - kolekcja zapewniająca szybki odczyt elementów na podstawie klucza
- Kolejka (`Queue<T>`) - jak sama nazwa wskazuje, kolekcja pokrywa funkcjonalności kolejki w konfiguracji FIFO
- Stos (`Stack<T>`) - pokrywa funkcjonalność stosu, dla uproszczenia można przyjąć, że działa tak samo jak kolejka w konfiguracji LIFO
- Zestaw (`HashSet<T>`) - Jest to hybryda listy i słownika, która zapewnienia unikalność dodawanych elementów, ale za cenę braku możliwości odwołania się do konkretnego elementu. Specyficznym rodzajem zestawu jest `SortedSet<T>`, który dodatkowo sortuje dodawane elementy.

**2. Czym się różni HashSet od listy lub tablicy?**

`HashSet<T>` jest kolekcją przechowującą unikalne elementy w nieokreślonym porządku, w przeciwieństwie do `List<T>` i `T[]`, które mogą posiadać duplikaty oraz są indeksowalne.

**3. Jakie jest przeznaczenie `Dictionary<T>` i czym się charakteryzują?**

Słownik jest kolekcją przechowującą pary klucz-wartość, gdzie każdy klucz musi być unikalny. Umożliwia to szybkie pobieranie wartości na podstawie klucza. Są bardzo często używane podczas mapowania lub operacji typu Lookup

**4. Czym jest operacja typu lookup w kontekście kolekcji?**

Jest to operacja, która odpowiada za proces wyszukiwania określonej wartości w kolekcji na podstawie jej klucze. Operacja ta różni się w zależności od typu używanej kolekcji:

- `Dictionary<T>` Lookup: w słownikach wyszukiwanie informacji odbywa się na podstawie klucze i pozwala na szybki odczyt
- `HashSet<T>` Lookup: Operacja typu lookup pozwala stwierdzić czy dany element istnieje w kolekcji czy też nie. Jest to szczególnie użyteczne kiedy nie potrzebujemy odczytać wartości, a jedynie dowiedzieć się czy istnieje, bądź nie
- `List<T>` or `T[]` Lookup: Listy i tablice nie posiadają operacji typu lookup w dosłownym tego słowa znaczeniu, natomiast posiadają możliwość odczytu informacji na podstawie indeksu.

Pamiętaj, że wydajność operacji typu lookup może się różnić w zależności od użytej kolekcji i trzeba je ostrożnie dobierać w zależności od wymagań.

**5. W jaki sposób dodawane są elementy do List w C#?**

Aby dodać element do listy należy użyć metody `Add()`:

```csharp
List<int> numbers = new List<int>();
numbers.Add(1);
numbers.Add(2);
```

Wewnętrznie klasa `List<T>` przechowuje tablicę elementów `T[]`, która najczęściej na początku ma rozmiar 4 (chyba, że programista zdecyduje inaczej). Następnie przy operacji dodawania `Add(T item)` kolejny element umieszczany jest w kolejnym wolnym indeksie tablicy. W przypadku gdy maksymalny rozmiar został osiągnięty wewnętrzna tablica zwiększa swój rozmiar (najczęściej dwukrotnie), a następnie poprzednia tablica kopiowana jest do nowoutworzonej i dodawany element umieszczany jest w pierwszym wolnym miejscu.

**6. W jaki sposób działa indeksowanie w tablicach?**

- Alokacja pamięci - Inicjalizując tablicę program przypisuje odpowiednią ilość pamięci w zależności od zadeklarowanego rozmiaru, gdzie ilość pamięci zależna jest od długości tablicy i typu przechowywanego w tablicy.
- Elementy - Elementy przechowywane są w ciągłym obszarze pamięci a dostęp do nich odbywa się poprzez indeks
- Kalkulacja obszaru pamięci - wiedząc, spod którego indeksu wartość ma zostać wyciągnięta i znając rozmiar pojedynczego bloku możesz z łatwością obliczyć dokładne miejsce w pamięci pod którym znajduje się szukany element
- Bezpośrednie odwołanie - znając dokładny obszar w pamięci możesz odczytać wartość w czasie o złożoności O(1) (to znaczy, w takim, że czas operacji jest niezależny od rozmiaru tablicy).

**7. Na jakiej podstawie określane są elementy do usunięcia z Listy?**

Metoda `Remove()` w `List<T>` zależy od wyniku metody `Equals` dla typu `T`. Metoda `Remove()` bierze przekazany parametr i przechodzi po każdym elemencie Listy i wywołuje metodę `Equals`. W przypadku napotkania pasującego elementu usuwa go i kończy swoje działanie.

UWAGA: Metoda `Remove()` usunie tylko pierwszy napotkany element spełniający równość.

UWAGA: Domyślan implementacja metody `Equals` dla klas porównuje referencje.

**8. W jaki sposób usunąć z Listy wszystkie elementy spełniające warunek?**

Można to zrobić korzystając z metody `RemoveAll` przekazując wyrażenie lambda jako `Predicate<T, bool>`, które wskaże elementy, które powinny zostać usunięte

**9. Jaka jest różnica pomiędzy `IEnumerable<T>` oraz `ICollection<T>`?**

Interfejs `IEnumerable<T>` jest najbardziej podstawowym służącym do iterowania po elementach kolekcji. Z drugiej strony `ICollection<T>`, który dziedziczy po `IEnumerable<T>` jest jego rozszerzeniem, który umożliwia modyfikowanie kolekcji poprzez dodawanie, usuwanie elementów, czy sprawdzanie ich liczby.

**10. Co implikuje, że typ impelmentuje interfejs `IEnumerable<T>`?**

11. Jakie jest przeznaczenie interfejsu `IEnumerable<T>`?

12. Do czego służy interfejs `IEnumerator<T>`?

13. Do czego służy słowo kluczowe `yield`?

14. Czy `yield` może być używane w innych metodach niż te, które zwracają `IEnumerable<T>`?

15. Jakie jest znaczenie "leniwej ewaluacji" (Lazy Evaluation) przy użyciu słowa kluczowego yield?

**16. Jakie są różnice pomiędzy `Queue<T>` i `Stack<T>`?**

`Stack<T>` to stos, który działa według zasady LIFO (Last In, First Out), co oznacza, że ostatni element dodany jest pierwszy do pobrania. `Queue<T>` to kolejka, która działa według zasady FIFO (First In, First Out), co oznacza, że pierwszy element dodany jest pierwszy do pobrania.

**17. Kiedy należy użyć `HashSet<T>` zamiast `List<T>`?**

`HashSet<T>` powinien być używany, gdy chcesz przechowywać unikalne wartości i wykonywać operacje przynależności i usuwania z dużą wydajnością. `HashSet<T>` gwarantuje, że nie zawiera duplikatów, a metody takie jak `Contains`, `Add` i `Remove` działają w czasie stałym. Natomiast `List<T>` może zawierać duplikaty i jest bardziej odpowiedni, gdy potrzebujesz zachować kolejność elementów i często wykonywać operacje na indeksach.

**18. Co to jest `Dictionary<TKey, TValue>` i jakie są sposoby dodawania elementów do słownika?**

`Dictionary<TKey, TValue>` to kolekcja, która przechowuje pary klucz-wartość, gdzie klucze są unikalne. Sposoby dodawania elementów do słownika to:

- Wykorzystanie indeksatora: `dictionary[key] = value`;
- Wykorzystanie metody Add: `dictionary.Add(key, value)`;
- Wykorzystanie metody `TryAdd`, która dodaje element tylko wtedy, gdy klucz nie istnieje: `dictionary.TryAdd(key, value)`;
