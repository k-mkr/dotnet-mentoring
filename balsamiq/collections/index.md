# Kolekcje

Platforma .NET zapewnia standardowy zestaw typów kolekcji, które pozwalają na obsługę dowolnych zbiorów danych. Wśród nich możemy znaleźć listy o zmiennym i stałym rozmiarze, słowniki, jak również bardziej specyficzne jak kolejki, czy stosy. Rodzajów jest cała masa i tylko od wymagań zależy, który z nich powinien zostać użyty.

Głównym czynnikiem przemawiającym za wykorzystywaniem kolekcji jest ich skuteczność i wydajność. Zostały przez twórców zaprojektowane tak, aby były wydajne i efektywne. Pozwalają na przechowywanie i zarządzanie dużymi zbiorami danych. Dodatkowo dostarczają szereg metod, który pokrywają większość zapotrzebowania użytkownika (dodawanie, usuwanie itd.). Kolejnym ważnym czynnikiem jest bezpieczeństwo typów, które jak już pewnie zdążyłeś się przekonać jest podstawowym wymaganiem języka C#. Dzięki temu masz pewność, że obiekty przechowywane w kolekcji będą zgodne w zakresie typu.

Każda z kolekcji implementuje pewien zestaw interfejsów, który definiuje ich zachowania.

Do najpopularniejszych typów kolekcji należą:

1. Lista (`List<T>`) - kolekcja o zmiennym rozmiarze, jest jedną z najbardziej uniwersalnych
2. Tablica (`T[]`) - kolekcja o stałym rozmiarze, które deklarujemy podczas inicjalizacji
3. Słownik (`Dictionary<TKey, TValue>`) - kolekcja zapewniająca szybki odczyt elementów na podstawie klucza
4. Kolejka (`Queue<T>`) - jak sama nazwa wskazuje, kolekcja pokrywa funkcjonalności kolejki w konfiguracji FIFO
5. Stos (`Stack<T>`) - pokrywa funkcjonalność stosu, dla uproszczenia można przyjąć, że działa tak samo jak kolejka w konfiguracji LIFO
6. Zestaw (`HashSet<T>`) - Jest to hybryda listy i słownika, która zapewnienia unikalność dodawanych elementów, ale za cenę braku możliwości odwołania się do konkretnego elementu. Specyficznym rodzajem zestawu jest `SortedSet<T>`, który dodatkowo sortuje dodawane elementy.

**Linki:**

- [MSDN Collections (C#)](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/collections)
- [Ciekawy Ebook o kolekcjach (strony 1-6)](https://dev-hobby.pl/wordpress/wp-content/uploads/wpcfto_files/c91d5f5cff94bdb8475108b7dae84e52book.pdf)
