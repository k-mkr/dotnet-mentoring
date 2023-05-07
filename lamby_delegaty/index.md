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

Myślę, że najlepiej wyjaśnić
