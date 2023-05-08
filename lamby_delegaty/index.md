# Delegaty i Wyrażenia Lambda

W poprzednim rozdziale poznaliśmy typy generyczne, które odegrają niebagatelną rolę w rozdziale dotyczącym wyrażeń lambda i obsługi zdarzeń. Delegaty i wyrażenia lambda to jedne z najpotężniejszych i najważniejszych mechanizmów języka C#. Pozwalają one na tworzenie elastycznych i modułowych aplikacji poprzez umożliwienie przekazywania metod jako parametrów innych metod co umożliwia definiowanie zdarzeń oraz ich obsługi.

Do tej pory poznaliśmy różne mechanizmy pisania elastycznego kodu, którego implementacja zmieniała się w trakcie działania programu jak np. dziedziczenie i interfejsy. Wyrażenia lambda i delegaty są właśnie takimi mechanizmami, a tym co je wyróżnia w stosunku do powyższych jest prostota i dowolność stosowania.

W tym module dowiesz się co to są delegaty i wyrażenia lambda, jakie są między nimi różnice oraz jakie jest ich wykorzystanie w praktyce. Powiemy sobie w jaki sposób delegaty i wyrażenia lambda pomagają pisać bardziej efektywny i rozszerzalny kod.

## 1. Delegaty

Delegat jest type referencyjnym, który umożliwia przekazywanie funkcji jako parametru do innych metod lub klas. Delegat jest bardzo podobny do wskażnika na funkcję występujący w innych językach programowania przy czym spełnia podstawowe założenie języka C#, a mianowicie zachowuje bezpieczeństwo typów - czyli może przechowywać referencję do metody, która jest zgodna z jego sygnaturą (typem zwracanym i listą argumentów).

Najpierw przeanalizujmy definicję i użycie delegata na rzeczywistym przykładzie, a dopiero później przyjrzymy się teorii i definicjom. Załóżmy, że mamy klasę `Calculator`, która w swojej podstawowej wersji obsługuje 4 działania - "+", "-", "\*", "/". Gdybym Cię teraz poprosił o napisanie takiej klasy mogłaby ona wyglądać następująco:

```csharp
public class Calculator
{
    public double Add(double x, double y) => x + y;

    public double Subtract(double x, double y) => x - y;

    public double Multiply(double x, double y) => x * y;

    public double Divide(double x, double y) => x / y;
}
```

Przy czym każde kolejne rozszerzenia wymagałyby modyfikacji tej klasy i implementacji dodatkowych operacji. A co jeśli pojawiłaby się potrzeba zapisywania wyniku przed zwróceniem do listy operacji historycznych? Wtedy kod mógłby wyglądać następująco:

```csharp
public class Calculator
{
    private List<double> _historicalResults = new List<double>();

    public double Add(double x, double y)
    {
        double result = x + y;
        _historicalResults.Add(result);
        return result;
    }
    ...
}
```

Patrząc na powyższe metody możemy zauważyć pewien wzorzec, a mianowicie,że każda metoda ma bardzo podobną sygnaturę - `double nazwa_metody(double x, double y)`. A co gdybyśmy mogli zamknąć obsługę kalkulatora w pojedynczej metodzie i przenieść definicję operacji na klienta? W tym mogą nam pomóc deletagy, które pozwolą zdefiniować sygnaturę metody, która ma zostać przekazana z zewnątrz do naszego kalkulatora:

```csharp
public delegate double Operation (double x, double y);

public class Calculator
{
    private List<double> _historicalResults = new List<double>();

    public double Calculate(Operation operation, double x, double y)
    {
        double result = operation(x, y);
        _historicalResults.Add(result);
        return result;
    }
}
```

Użycie klasy `Calculator` wyglądałoby następująco:

```csharp
public static void Main()
{
    Calculator calculator = new Calculator();

    Operation add = Add;

    double result = calculator.Calculate(add, 10, 20);
}

public static double Add(double x, double y) => x + y;
```

Egzemplarz delegatu `add` pełni funkcję przedstawiciela, który wywołuje delegate, aby za jego pośrednictwem została wywołana metoda docelowa. Dzięki temu wyeliminowaliśmy zależność klasy `Calculator` od zmiennych założeń wywołującego.

Na podstawie powyższego przykładu możemy określić podstawowe założenie delegatów:

- usuwanie zależności z klasy i przenoszenie ich na wywołującego
- delegate są swego rodzaju wsakźnikami do funkcji (typami referencyjnymi) co pozwala nam na przekazywanie go w klasach, metodach i korzystać z niego w różnych miejsach kodu.
- delegaty umożliwiają odseparowanie logiki od interfejsu użytkownika co znacząco ułatwia rozszerzanie i testowanie kodu.

**Definicja:**

```csharp
delegate <typ_zwrotny> nazwa_delegatu (<parametry_wejściowe>);
```

## 2. Zaawansowane zagadnienia związane z delegatami

### 2.1. Pisanie metod wtyczek

Definicja delegatu jest typem referencyjnym co pozwala na wykorzystanie wszystkich zalet z tego wynikających. Metody do zmiennych delegacyjnych są przypisywane w czasie działania programu. Wykorzystując ten fakt możemy pisać metody wtyczki (tzw. plug-in methods), które będą mieć stałą logikę wewnętrzną, która będzie się zmieniać w zależności od metod przypisanych do delegatów.

Myślę, że najlepiej zrozumiesz to na rzeczywistym wymaganiu biznesowym, które sobie rozpatrzymy.

Wyobraź sobie, że tworzysz system do finalizacji zamówień w sklepie internetowym. Cześcią, którą się obecnie zajmujesz jest wyliczanie końcowej kwoty, którą użytkownik będzie musiał zapłacić za zamówienie. Na całą kwotę składają się następujące wartości:

- wartość koszyka
- cena wysyłki
- wykorzystanie kodu rabatowego

Dodatkowo chcielibyśmy umożliwić aby użytkownik mógł logować poszczególne wartości do dowolnie wybranego przez siebie miejsca w celu późniejszego sprawdzenia poprawności obliczeń

Jako producent oprogramowania chcesz dostarczać klientowi możliwie najbardziej rozszerzalny produkt, który będzie mógł sobie dostosowywać według potrzeb. Dla przykładu różni klienci mogą mieć różne kody rabatowe i ich wartości jak również kwota, od której dostawa będzie darmowa również moze być różna. Do zaprojektowania takiej funkcjonalności możemy wykorzystać delegaty:

```csharp
public delegate double ApplyPromo(double price, string discountCode);

public delegate double DeliveryFee(double price, string address);

public delegate void LogPriceCalculation(double shoppingCartValue, double discount, double deliveryFee, double endPrice);

public class OrderSummary
{
    private double _shoppingCartValue;
    private string _discountCode;
    private string _deliveryAddress;

    public double CalculateEndPrice(ApplyPromo discount, DeliveryFee deliveryFee, LogPriceCalculation logging)
    {
        double endPrice = _shoppingCartValue;

        double discountValue = discount(_shoppingCartValue, _discountCode);

        double deliveryFeeValue = deliveryFee(endPrice, _deliveryAddress);

        endPrice = endPrice - discountValue + deliveryFeeValue;

        logging(_shoppingCartValue, discountValue, deliveryFeeValue, endPrice);

        return endPrice;
    }
}
```

Jak widzisz na poniższym przykładzie całą logikę odpowiedzialną za obliczanie ceny końcowej, którą klient będzie zmuszony zapłacić za zamówienie przenieśliśy na użytkownika naszego systemu, który ma teraz pełną dowolność co do sposobu aplikowania kodów promocyjnych, opłaty za przesyłkę jak również miejsca gdzie zostaną zalogowane obliczenia. Dla odmiany ty jako producent możesz dowolnie modyfikować ciało metody `CalculateEndPrice` i dopóki sygnatura delegat i samej metody się nie zmieni możesz do niej wprowadzać nowe funkcjonalnośći, bez potrzeby informowania o nich użytkownika. Przyjrzyjmy się jak mógłoby wyglądać przykładowe użycie klasy `OrderSummary` przez dwie różne firmy - Allegro i Amazon.

```csharp
public class AmazonOrder
{
    private double _freeDeliveries = 100;

    private OrderSummary _order;

    public double MakeCalculation()
    {
        return _order.CalculateEndPrice(BlackWeekPromo, FreeDeliveriesForPolandPromotion, WriteToFile);
    }

    public double BlackWeekPromo(double price, string code)
    {
        if (code == "BLACKWEEK")
            return price * 0.2;

        return price;
    }

    public double FreeDeliveriesForPolandPromotion(double price, string address)
    {
        if (_freeDeliveries > 0 && address.Contains("Poland"))
            return 0;

        return 15.5;
    }

    public void WriteToFile(double shoppingCartValue, double discount, double deliveryFee, double endPrice)
    {
        // Write to file
    }
}

public class AllegroOrder
{
    private OrderSummary _order;

    public double MakeCalculation()
    {
        return _order.CalculateEndPrice(EmptyPromotion, SmartDeliveryFee, LogIntoDatabase);
    }

    public double EmptyPromotion(double price, string code)
    {
        return price; // no promo
    }

    public double SmartDeliveryFee(double price, string address)
    {
        if (price >= 40)
            return 0;

        return 10;
    }

    public void LogIntoDatabase(double shoppingCartValue, double discount, double deliveryFee, double endPrice)
    {
        // Write to database
    }
}
```

Jak widzisz obie implementacje są od siebie całkowicie niezależne i korzystają z zaimplementowanej przez Ciebie logiki. Gdyby teraz pojawiało się wymaganie biznesowe, że chcemy zbierać statystyki co do miejsc, z których wykonywane są zamówienia, aby później móc podejmować decyzje na podstawie tych danych to jedyne co wystarczyłoby zrobić to zmodyfikować metodę `OrderSummary.CalculateEndPrice` o stosowną implementację i wszystko odbywa się przezroczyście dla klientów tej metody.

Na podstawie powyższego przykładu możesz się zastanawiać - No OK, ale dlaczego nie wykorzystać do tego dziedziczenia:

```csharp
public abstract class OrderSummaryBase
{
    ...

    public double CalculateEndPrice(ApplyPromo discount, DeliveryFee deliveryFee, LogPriceCalculation logging)
    {
        double endPrice = _shoppingCartValue;

        double discountValue = Discount(_shoppingCartValue, _discountCode);

        double deliveryFeeValue = DeliveryFee(endPrice, _deliveryAddress);

        endPrice = endPrice - discountValue + deliveryFeeValue;

        Logging(_shoppingCartValue, discountValue, deliveryFeeValue, endPrice);

        return endPrice;
    }

    protected abstract double Discount(double price, string discountCode);

    protected abstract double DeliveryFee(double price, string address);

    protected abstract double Logging(double shoppingCartValue, double discount, double deliveryFee, double endPrice);
}
```

I tak masz rację, ale zauważ, że w tym wypadku tracisz całą elastyczność wynikającą z możliwości dostarczania dowolnych implementacji z zewnątrz. A gdyby teraz Ci powiedział, że jeden z użytkowników chciałby robić drop-shopping na Allegro produktami z Amazonu? Wymagałoby to kolejnego dziedziczenia i duplikacji kodu w celu zapewnienia funkcjonalności z obu sklepów jednocześnie narażając się na błędy, gdzie przy wykorzystaniu delegat problem możnaby rozwiązać następująco:

```csharp
public class DropShoppingOrder
{
    private AmazonOrder _amazonOrder;
    private AllegroOrder _allegroOrder;

    private OrderSummary _order;

    public double CalculateEndPrice()
    {
        return _order.CalculateEndPrice(_amazonOrder.BlackWeekPromo, _allegroOrder.SmartDeliveryFee, LoggingOwnSystem);
    }

    private void LoggingOwnSystem(double shoppingCartValue, double discount, double deliveryFee, double endPrice)
    {
        // write to own system
    }
}
```

Tego typu rozszerzenia nie byłyby trywialne przy pomocy zwykłych mechanizmów dziedziczenia albo skończyłyby się kodem, który jest słabo utrzymywalny i bardzo kruchy.

### 2.2 Delegaty multiemisji

Kolejną funkcjonalnością, która wyróżnia delegaty spośród innych mechanizmów przenoszenia odpowiedzialności jest ich zdolność do multiemisji (ang. multicasting). Oznacza to nic innego jak to, że egzemplarz delegatu może odnosić się do więcej niż jednej metody, ale do całej ich listy. Do operacji na delegatach używamy standardowych operatorów - +=, -=, =. Działanie tego mechanizmu ponownie pokażemy na przykładzie.

Wyobraźmy sobie, że jesteśmy właścicielami oprogramowania, które służy do przetwarzania i kompresowania danych zbieranych przez satelity umieszczone w kosmosie. Nasze oprogramowanie wspiera różne instytucje jak NASA, czy SpaceX, ale jest również dostępne dla zwykłych pasjonatów.
Każdy z klientów chciałby na swój sposób monitorować postęp przetwarzania. W tym celu można wykorzystać mechanizm multiemisji:

```csharp
public delegate void ProgressMonitor(int processed, int totalCount);

public class DataProcessor
{
    public ProgressMonitor Monitor;

    public void Process(List<SpaceData> data)
    {
        for(int i = 0; i < data.Count; ++i)
        {
            data[i].Process();
            Monitor(i, data.Count);
        }
    }
}
```

Powyższa prosta implementacja przedstawia proces przetwarzania listy danych `SpaceData`. Implementacja tej klasy została pominięta, gdyż nie jest przedmiotem przykładu. Klasa `DataProcessor` udostępnia publiczną właściwość o typie delegaty, do której może się odwołać każdy klient i dzięki temu monitorować postęp przetwarzania. Przykładowe użycie mogłoby wyglądać następjąco:

```csharp
var processor = new DataProcessor();

processor.Monitor += NasaMonitor;
processor.Monitor += SpaceXNotifier;

processor.Process(data);

void NasaMonitor(int processed, int totalCount)
{
    Console.WriteLine($"Sent to NASA Server. {processed}/{totalCount}");
}

void SpaceXNotifier(int processed, int totalCount)
{
    if (processed == totalCount)
        Console.WriteLine("Data was processed.");
}
```

Przy za każdym razem kiedy wywołamy delegat `Monitor` wszystkie metody, które zostały do niego dodane zostaną wywołane **w kolejności dodawania**. Jak widać implementacja wewnętrzna tych metod może być dowolna i tak w tym przypadku NASA jest zainteresowana ciągłym monitoringiem, a dla odmiany SpaceX chce jedynie dostać informacje kiedy przetwarzanie zostanie zakończone.

Jeśli chcemy możemy również usuwać delegaty w trakcie działania programu przy pomocy operatora -=. Operacja ta jest idempotentna czyli jeśli dany delegat nie istnieje w naszej liście delegatów nic się nie stanie.

**Uwaga:** Delegaty są niezmienne, mimo że są typami referencyjnymi to operacje += i -= w rzeczywistości tworzą nowy egzemplarz delegata (nowe miejsce w pamięci) i przypisują go do istniejącej zmiennej.

Może się teraz zastanowić wszystko fajnie, ale co jeśli delegat ma inny typ zwrotny niż `void`? W tym przypadku standardowo zostaną wykonane wszystkie funkcje w kolejności dodania przy czym wynik zostanie zwrócony tylko dla ostatniej wykonanej metody. Reszta wartości zostanie zwyczajnie zignorowana.

Podsumowując multiemisja delegatów może być używana przy rozwiązywaniu następujących problemów:

- Obsługa zdarzeń, będziemy ten temat poruszać w dalszej cześci tego rozdziału, przy czym dzięki multiemisyjności możemy wywoływać różnych i niezależnych od siebie metod po wystąpieniu określonego zdarzenia.
- Multiemisyjność delegatów może być użyta do implementacji wzorca projektowego _Polecenie_ (ang. Command), o którym będziemy mówić w dalszej części nauki
- Zastosowanie multiemisyjności można również znaleźć przy implementacji metod typu Plug-In gdzie w trakcie działania programu możemy dodawać kolejne akcje które zostaną wykonane jedna po drugiej w odpowiedniej chwili
- Kolejną gałęzią jest programowanie asynchroniczne, gdzie możemy wywołać kilka metod asynchronicznych (działających w tle) i czekać na ich zakończenie.

## 3. Standardowe delegaty Func i Action

Typy delegacyjne nie mają ograniczeń i aplikują się do nich wszystkie dotychczas poznane przez Ciebie zagadnienia programowania obiektowego. Dla przykładu nic nie stoi na przeszkodzie abyś typ delegacyjny posiadał generyczne parametry typów. Dla przykładu:

```csharp
public delegate TDestination Transform<TSource, TDestination>(TSource source, TDestination destination);
```

Zauważ, że pisanie delegatów może być czasem karkołomne i wymagać dużej ilości nowych typów. Dodatkowo każda z klas posiadałaby swój zestaw delegat, a ich sygnatury nakładałyby się na siebie w projekcie co zmniejszałoby czytelność i utrzymanie kodu.

Aby pozbyć się tego problemu i zunifikować użycie typów delegujących wprowadzono dwa standardowe delegaty generyczne, które pozwalają na pisanie delegat działających z metodamy o każdym typie zwrotnym i dowolnej liczbie argumentów (ograniczenie ustawiono do 16 parametrów).
Tymi typami są delegaty Func i Action, które są zdefiniowane w stanardowej przestrzeni nazw i są domyślnie dostępne dla programisty.

### 3.1. Delegat Action

Możesz się zastanowić dlaczego wprowadzono dwa różne delegaty w celu pokrycia wszystkich możliwych wariancji sygnatur metod. Główną przyczyną jest typ zwrotny `void`, dla którego nie ma odpowiednia generycznego. Delegat `Action` pokrywa wszystkie sygnatury, które mają typ zwrotny void. Poniżej przedstawiam definicję typu `Action`:

```csharp
delegate void Action<in T>(T arg);
delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
... // aż do T16
```

Tych wszystkich delegatów nie trzeba definiować ręcznie, ponieważ są już wbudowane w język i dostępne w przestrzeni nazw `System`. I tak użycie takiego delegatu mogłoby wyglądać następująco:

```csharp
PrintRandom(PrintConsole); // Console: <number>

PrintRandom(PrintFile); // <number> in temp file

void PrintRandom(Action<int> printAction)
{
    Random rnd = new Random();
    printAction(rnd.Next());
}

void PrintConsole(int number)
{
    Console.WriteLine($"Console: {number}");
}

void PrintFile(int number)
{
    string tempFile = Path.GetTempFileName();
    File.WriteAllText(tempFile, number.ToString());
}
```

Dzięki zastosowaniu delegata `Action` kod jest czytelniejszy i osoba, która korzysta z metody, która wymaga parametru `Action` jak argument od razu wie z czym ma do czynienia i jak z tym typem pracować.

### 3.2. Delegat Func

Delegat `Func` jest bardzo podobny do wyżej omawianego `Action` z tą różnicą, że delegat `Func` pokrywa sygnatury z typem zwracanym. Poniżej przedstawiam definicje tych delegatów, które również są już wbudowane w język.

```csharp
delegate TResult Func<out TResult>();
delegate TResult Func<in T, out TResult>(T arg);
delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
...
```

Przykład użycia delegatu `Func` jest taki sam jak typu `Action`, ważne jest, aby sygnatura się zgadzała. Dla przećwiczenia pokażę Ci zastosowanie delegatu `Func` w funkcjach LINQ, których będziemy się uczyć w dalszych rozdziałach.

Używając LINQ możemy przefiltrować listę, aby zwracała tylko wartości spełniające dany warunek. I tak sygnatura takiej wbudowanej w język metody to:
