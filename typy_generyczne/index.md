# TYPY GENERYCZNE

Typy generyczne są jednym z najbardziej użytecznych mechanizmów języka C#, który obok interfejsów i dziedziczenia pozwala na pisanie bardziej elastycznego i czytelnego kodu będącego jednocześnie mniej podatnym na błędy i powtórzenia.

Cechą charakterystyczną dla typów generycznych jest możliwość działania tych samych klas i metod z różnymi typami bez utraty bezpieczeństwa typów, które jak już wiesz jest podstawowym założeniem języka C#. W tym module dowiesz się po co są typy generyczne oraz jakie są zalety ich używania na rzeczywistych przykładach. Dodatkowo pokażę Ci w jaki sposób można łączyć typowanie generyczne z dotychczas poznanymi mechanizmami programowania obiektowego (dziedziczenie, interfejsy). Nauczysz się też wykorzystywać dodatkowe funkcje projektowania typów generycznych jak ograniczenia, które pozwalają na pisanie spójnego kodu zgodnego z oczekiwaniami autora.

Na koniec tego modułu chciałbym żebyś miał pełny przegląd co do użycia typów generycznych oraz żebyś czuł się z nimi komfortowo.

**Od Autora:** Wraz z doświadczeniem i obyciem z kodem typy generyczne staną się naturalnym narzędziem w Twoim wachlarzu, a ich użycie będzie się samo narzucało.

## Definicja

Zacznijmy od podstawowego pytania: co to jest typ generyczny?

Typ generyczny to taki typ (najczęściej klasa), który może przyjść dowolny inny typ jako parametr. Dzięki temu można tworzyć metody/klasy pracujące z różnymi typami danych, nie tracąc przy czym na bezpieczeństwie typów i wydajności.

Dzięki temu można opóźnić dostarczenie "konkretnego typu danych" do momentu użycia w trakcie wykonywania programu. Dzięki temu można tworzyć "ogólny" typ, który z każdym użyciem może być "wypełniony" innym typem. Chcesz żeby jeden egzemplarz używał "int", a drugi "string"? Nie ma sprawy!

Spróbujmy teraz pobawić się trochę kodem i zobaczyć jak działają typy generyczne w praktyce. Weźmy na warsztat informatyczną firmę spedycyjną, która zajmuje się pakowaniem i wysyłką różnych paczek z danymi. Tymi danymi mogą być liczby, stringi, a nawet klasy - pełny przekrój, full-service!

Oczywiście funkcjonalność ta może być pokryta przy użyciu standardowych mechanizmów programowania, które już znasz:

```csharp
public class PackageInt
{
    public int Data { get; set; }

    public void Pack(int data)
    {
        Data = data;
    }
}

public class PackageString
{
    public string Data { get; set; }

    public void Pack(string data)
    {
        Data = data;
    }
}
...
```

Myślę, że już sam widzisz problem z tym podejściem. Rozwój powyższego kodu obarczony jest dużym ryzykiem błędu oraz potrzebą duplikacji kodu, a co jeśli będziemy chcieli wyciągnąć daną z paczki? Każda z klas musiałaby być zmieniona i przetestowana. Wymaganie co do obsługi kolejnego typu wymagają od nas napisania i przetestowania kolejnej klasy.

Z drugiej strony wiesz już, że istnieje typ nadrzędny nad wszystkimi innymi - `object`, który umożliwi Ci napisanie jednej implementacji działającej dla wszystkich typów:

```csharp
public class Package
{
    public object Data { get; set; }

    public void Pack(object data)
    {
        Data = data;
    }
}
```

Pozwól, że wady tego podejścia omówię w osobnym podrozdziale dotyczącym zalet i wad typów generycznych, ponieważ dla wielu osób, które spotykają się z typami generycznymi po raz pierwszy użycie typu `object` jest naturalnym odruchem.

### Deklaracja Typu Generycznego

Powyższą funkcjonalność można pokryć przy użyciu typów generycznych:

```csharp
public class Package<T>
{
    public T Data { get; set; }

    public void Pack(T data)
    {
        Data = data;
    }
}
```

**Definicja:** Typ generyczny deklaruje **parametry typów** - typy zastępcze, które powinny zostać zastąpione konkretnymi typami przez użytkownika dostarczającego **argumenty typów**. Typy generyczne deklarujemy tak samo jak klasy, a parametr typów przekazywane są w nawiasach "ostrych".

Poniżej przedstawiony jest typ generyczny `Package<T>` z powyższego przykładu, który służy do przechowywania danych typu `T`. Typ ten zawiera deklarację jednego parametru typu o nazwie T.

Typ ten może być użyty następująco:

```csharp
Package<string> packageWithString = new Package<string>();
packageWithString.Pack("test");

Package<int> packageWithInt = new Package<int>();
packageWithInt.Pack(10);

packageWithInt.Pack("ten"); // błąd kompilacji
```

W definicji `Package<int>` w miejsce typu T podano argument typu `int` co spowodowało niejawne utworzenie typu w locie (tworzenie następuje już w trakcie działania programu). Próba wstawienia łańcucha do typu `Package<int>` zakończy się błędem kompilacji. Dla lepszego zobrazowania "tworzenia typu w locie" możemy sobie wyobrazić, że utworzenie typu `Package<int>` kompilator interpretuje tak jakby użytkownik napisał następujący kod (nazwa klasy została zmieniona dla uproszczenia):

```csharp
public class Package_INT
{
    private int _data;

    public void Pack(int data)
    {
        _data = data;
    }
}
```

Zgodnie z teorią języka `Package<T>` nazywamy **typem otwartyn**, a `Package<int>` **typem zamkniętym**. W trakcie działania programu wszystkie egzemplarze są typami zamkniętymi, tzn. konieczne jest zawsze podanie konkretnego typu, który zostanie użyty. Poniższy kod zgłosi błąd kompilacji (jedyny wyjątek jeśli gdzieś użytkownik zadeklarował typ `T`):

```csharp
Package<T> package = new Package<T> // co to jest T?
```

**Dobre praktyki:** Zgodnie z dobrymi praktykami parametry typów najczeście oznaczamy literami T+, czyli T, U, V... W zależności od przeznaczenia parametru typu można też używać bardziej opisowej nazwy, przy czym warto ją poprzedzić literą T, wtedy jednoznacznie sugerujemy, że dany typ jest parametrem typu np. `class Loader<TSource>`, `class Write<TSource, TDestination>`.

## Zalety i wady typów generycznych

Zanim przejdziemy do możliwości zastosowania typów generycznych przyjrzyjmy się ich pozytywnym jak i negatywnym aspektom.

1. **Reużywalność kodu** - Dzięki typom generycznym możemy napisać kod, który działa dla wielu różnych typów danych, co pozwala nam na ponowne wykorzystanie kodu i uniknięcie pisania osobnych metod dla każdego typu danych. Nietrudno sobie wyobrazić jak wiele byłoby duplikatów kiedy musielibyśmy pisać osobną implementację dla każdego nowowymaganego typu danych

2. **Hermetyzacja Typu w obrębie klasy** - Zauważ, że w przypadku klasy posiadającej więcej niż jedną metodą, przy użyciu metody generycznej zapewniamy, że wszystkie metody wywołane na egzemplarzu tej klasy będą wymagały tego samego argumentu typu:

   ```csharp
   List<string> list = new List<string>();

   list.Add("test");
   list.Remove("test");
   list.Remove(1); // błąd kompilacji - lista jest typu zamkniętego List<string>
   ```

3. **Sprawdzenie typów podczas kompilacji** - Typy generyczne pozwalają zachować bezpieczeństwo typów. Przekazane typy są sprawdzane pod kątem poprawności typów już na etapie kompilacji, dzięki czemu błędy wynikające z użycia niepoprawnego typu są wykrywane już na etapie pisania kodu. Dzięki temu oszczędzamy czas, który musielibyśmy poświęcić na debugowanie i poprawianie kodu. Jako przykład wykorzystajmy klasę `Package` opartą na użyciu typu `object`. Poniższy kod zakończy się błędem w trakcie działania programu, przy użyciu typów generycznych błąd zostałby wychwycony przez kompilator.

   ```csharp
   package.Pack(10);
   string ten = (string)package.Data; // błąd wykonawczy - nie da się zrzutować int -> string
   ```

4. **Wydajność** - Typy generyczne nie wymagają operacji boxingu/unboxingu oraz rzutowania typów podczas działania na różnych typach danych co ma przełożenie na zwiększoną wydajność kodu i mniejsze zużycie pamięci. Dla zobrazowania problemu weźmy ponownie na warsztat naszą klasę `Package` z wykorzystaniem typu `object`. Poniższy kod zawiera dodatkowy narzut czasu działania procesora przy pakowaniu i rozpakowywaniu oraz konwersji. Dodatkowo zauważ, że zużywamy więcej pamięci, ponieważ jak już wiesz zmienne referencyjne odkładane są na stercie.

```csharp
Package package = new Package();
package.Pack(10); // pakowanie typu wartościowego int -> typ referencyjny object
int ten = (int)package.Data; // rzutowanie w dół object -> int
```

## Zastosowanie typów generycznych

TBD
