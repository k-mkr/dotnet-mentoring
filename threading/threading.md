# Programowanie asynchroniczne

## Wątki

Wielowątkowość - ogólny mechanizm pozwalający programowi na równoległe wykonywanie kodu. Jest obsługiwana zarówno przez środowisko uruchomieniowe CLR jak i system operacyjny, stanowi podstawową koncepcję współbieżności.

Wątek (Thread) - ścieżka wykonania, która może być realizowana niezależnie od innych ścieżek. Jest to niskopoziomowe narzędzie dostarczane przez środowisko CLR.

```csharp
Thread t = new Thread(() => Write('y'));
t.Name = "Sample Thread"; // nazywanie wątków ułatwia debugowanie i analizę kodu
//Thread.CurrentThread.Name -> nazwa aktualnie wykonywanego wątku

t.Start(); // t.IsAlive = true

Write('x');

static void Write(char c)
{
    for(int i = 0; i < 100; ++i)
    {
        Console.Write(c);
    }
}

// Output:
// yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxyyyyyyyyyyyyyyyyyyy
```

W komputerze jednordzeniowym system operacyjny musi zalokować "wycinki" czasu dla poszczególnych wątków (zwykle 20ms w systemie Windows), aby symulować współbieżność. Sytuacja jest analogiczna jeśli jest więcej wątków niż rdzeni procesora

Wywłaszczenie wątku - miejsca/okresy gdzie wykonywanie wątku jest przeplatane z wykonywaniem kodu innego wątku

UWAGA: Wątek kończy się jeśli przekazany do niego delegat kończy pracę. Raz uruchomiony wątek nie może być ponownie uruchomiony

Metoda `Join()` służy do poczekania, aż inny wątek zakończy pracę. Poniższy fragment kodu nie wyświetli "Koniec" dopóki `t` się nie skończy. Istnieje możliwość podania maksymalnego czasu oczekiwania. Metoda `Join()` zwraca `bool` w zależności czy udało się zakończyć działanie wątku w określonym czasie.

```csharp
Thread t = new Thread(Write('y'));
t.Start();
bool result = t.Join();

Console.WriteLine($"Koniec - {result}.");
// Output:
// yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
// Wątek t został zakończony - True
```

Metoda `Thread.Sleep()` powoduje wstrzymanie bieżącego wątku na podaną ilość czasu.

Zablokowany wątek natychmiast udostępnia (ang. yield) przydzielony mu czas procesora, aż do chwili spełnienia warunku powodującego blokadę.

UwAGA: `Thread.Sleep(0)` powoduje, że wątek natychmiast zrzeka się przydzielonego mu czasu i oddaje go procesorowi. Methoda `Thread.Yield()` działa tak samo, ale dotyczy tylko wątków działających w tym samym procesorze. Wywołanie tych metod pomaga w diagnostyce. Dodatkowo pomaga wykryć problem, bo jeśli umieszczenie metody w kodzie spowoduje awarie programu to prawie na pewno oznacza to istnienie w nim błędu.

Właściwość `ThreadState` pozwala na sprawdzenie stanu wątku. Wykorzystuje się wartości: Unstarted, Running, WaitSleepJoin, Stopped. Właściwość ta nie jest zalecana podczas synchronizacji, ponieważ stan wątku może ulec zmianie pomiędzy sprawdzenie, a użyciem otrzymanej informacji.

Przełączanie kontekstu (Spinning) - jest przeprowadzane przez system operacyjny podczas wywłaszczania wątku. Powoduje niewielkie obciążenie (1-2 us) oraz każdy nowy wątek zabier ~1MB pamięci. Należy mieć to na uwadze przy wielu krótkich operacjach, ponieważ może to obniżyć wydajność programu.

Operacja powiązana z wejściem-wyjściem (I/O) - operacja, w której większość czasu zajmuje oczekiwanie na pewne zdarzenie (pobranie strony internetowe, wywołanie `Console.ReadLine`). Każda tego typu operacja działa na dwa sposoby - synchronicznie (oczekiwanie na bieżący wątek aż do zakończenia operacji) lub asynchronicznie (uruchomienie wywołania zwrotnego po zakończeniu operacji w przyszłości). Synchroniczne operacje przez większość czasu blokują wątek co jest marnotrastwem czasu procesora, ponieważ według środowiska CLR wątek przeprowadza poważne obliczenia (otrzymuje zasoby) przez co staje się operacją powiązaną z obliczeniami.

Operacja powiązana z obliczeniami - większość czasu zajmują działania procesora

### Kontekst synchronizacji i Marshalling watku

**Marshalling** - zjawisko polegające na przekazaniu żądania z poziomu wątku roboczego do wątku interfejsu użytkownika w celu aktualizacji wartości. Często spotykane w bogatych aplikacjach klientów. Wątek interfejsu użytkownika posiada kolejkę komunikatów (ta sama, które np. obsługuje zdarzenia myszy zegrów), na którą odkładane są żądania z wątków roboczych. Dzięki temu wątki robocze delegują wykonanie kodu do wątku interfejsu użytkownika. Poniższy przykład ilustruję technikę marshallingu dla aplikacji WPF - `Invoke` (wykonaj i poczekaj na wynik), `BeginInvoke` (wykonaj i zapomnij). Dzięki temu użytkownik zawsze ma responsywne okno interfejsu.

```csharp
Thread t = new Thread(Work);
t.Start();

void Work()
{
    Thread.Sleep(1000); // Czasochłonna operacja
    Action send = () => textBox.Text = "Zrobione!";
    Dispatcher.BeginInvoke(send);
}
```

Klasa `SynchronizationContext` pozwala na uogólnienie pojęcia marshallingu wątku. Właściwość `SynchronizationContext.Current` pozwala na przechwycenie kontekstu, który zostanie użyty do przekazania żądania z wątku roboczego. `SynchronizationContext` w ASP .NET odgrywa dodatkowo subtelną rolę gwarantującą, że zdarzenia przetwarzania stron, będą przetwarzane sekwencyjnie po operacjach asynchronicznych, co ma na celu zachowanie kontekstu - `HttpContext`. Przekazanie żądania odbywa się poprzez wywołanie metod `Post` (`BeginInvoke`) oraz `Send` (`Invoke`).

```csharp
SynchronizationContext ctx = SynchronizationContext.Current;

new Thread(DoSomething)
    .Start();

void DoSomething()
{
    Thread.Sleep(1000);
    ctx.Post(_ => Console.WriteLine("Wiadomość wysłana..."), null);
}
```

### Wątki aktywne i działające w tle

Domyślnie tworzone wątki są **wątkami aktywnymi**. Program pozostaje w działaniu dopóki istnieje przynajmniej jeden wątek tego rodzaju. Inaczej wygląda sytuacja z **wątkami działającymi w tle**, które mogą zostać niedokończone jeśli wszystki wątki aktywne wcześniej zakończą działanie. Do zarządzania rodzajem wątku służy właściwość `IsBackground`.

UWAGA: Rodzaj wątku (aktywny, w tle) nie ma żadnego związku z jego priorytetem i czasem przydzielonym przez procesor.

W przypadku wątków działających w tle po zakończeniu działania aplikacji nie zostaną wywołane żadne operacje czyszczące jeśli były zdefiniowane (`finally`, `using`). Aby tego uniknąć należy wykorzystać konstrukcję opartą na sygnałach.

### Priorytet wątku

Właściwość `Priority` określa ile czasu procesora otrzyma wątek w porównaniu do innych wątków działających w systemie. Odbywa się to na pięciostopniowej skali typu wyliczeniowego `ThreadPriority` - Lowest, BelowNormal, Normal, AboveNormal, Highest. Należy być ostrożnym z manipulowaniem priorytetami, ponieważ może to negatywnie wpływać na inne wątki działające w systemie.

UWAGA: Jeśli chcemy aby wątek miał pierwszeństwo przed wątkami w innych procesach to musimy zwiększyć priorytet całego procesu - `Process.PriorityClass`. Tego typu manipulacje mogą oczywiście spowolnić działanie całego komputera.

### Stan lokalny vs współdzielony

Każdy wątek posiada własny stos pamięci, który pozwala na przechowywanie zmiennych lokalnych. Wątki współdzielą dane jeśli mają wspólne odwołanie do tego samego egzemplarza obiektu. Zmienne lokalne przechwycone przez wyrażenie lambda/delegat są przez kompilator konwertowane na elementy składowe i mogą być współdzielone.

Mechanizmy współdzielenia definiują koncepcję jaką jest **bezpieczeństwo wątków**. W przypadku operowania na tych samych danych rezultaty mogą być nieokreślone. Wynika to z tego, że jeden wątek może sprawdzać daną wartość i na tej podstawie wykonywać działania, kiedy drugi tą wartość zmieni zaraz po sprawdzeniu.

Powoduje to powstanie subtelnych błędów, które są niedeterministyczne co czyni je trudnymi do wykrycia. Rozwiązaniem na ten problem jest wykorzystywanie blokad, jednak najlepszą praktyką jest unikanie współdzielonego stanu gdzie tylko jest to możliwe.

Czasami zachodzi potrzeba przekazania argumentów do metody startowej wątku. Najłatwiej zrobić to poprzez przekazanie parametrów bezpośrednio do metody lambda. Można też wykorzystać stary mechanizm, który pozwala na podanie argumentu w metodzie `Start()`. Metoda przyjmuje tylko jeden argument o typie `object` co najczęściej wymaga rzutowania.

Przekazywanie argumentów bezpośrednio do metody lambda jest zalecanym sposobem, ale również tu należy zachować ostrożność żeby przypadkiem nie zmodyfikować przechwyconych zmiennych po uruchomieniu wątku. Jak widać na poniższym przykłądzie przekazana zmienna `i` odwołuje się do tego samego miejsca w pamięci w trakcie całego cyklu życiowego pętli. Rozwiązaniem byłoby przypisanie wartości `i` do zmiennej lokalnej. Każda wartość zostanie przypisana raz jednak kolejność dalej może być niedeterministyczna co wynika z tego, że wątki moga być uruchamiane w różnym czasie.

Podobny problem został zilustrowany w kolejnym przykładzie

UWAGA: Kiedyś tego typu problem występował w pętli foreach. Jednak został naprawiony i teraz każdy element przypisywany jest do innego miejsca w pamięci.

```csharp
for(int i = 0; i < 9; i++)
    new Thread(() => Console.Write(i)).Start();

for (int i = 0; i < 9; i++)
{
    int local = i;
    new Thread(() => Console.Write(local)).Start();
}

// Output:
// 223566889
// 012345678
```

```csharp
int i = 1;

Thread t1 = new Thread(() => Console.Write(i));

i = 2;

Thread t2 = new Thread(() => Console.Write(i));

t1.Start();
t2.Start();

// Output: 22
```

### Obsługa wyjątków

W związku z tym, że każdy wątek ma swoją ścieżkę wykonania wszystkie wyjątki, które wystąpią podczas działania nie zostaną przechwycone, a wątek będzie obciążony nieobsłużonym wyjątkiem. Rozwiązaniem tego problemu jest przeniesienie obsługi wyjątku do lambdy.

```csharp
try
{
    new Thread(() => throw new NullReferenceException()).Start();
}
catch (Exception)
{
    Console.WriteLine("Wyjątek!");
}

Console.WriteLine("Koniec.");

// Output:
// Koniec.
```

Nieobsłużone wyjątki mogą być zgłoszone przy zakończeniu programu:

```csharp
Koniec.
Unhandled exception. System.NullReferenceException: Object reference not set to an instance of an object.
   at Program.<>c.<<Main>$>b__0_0() in C:\_Work\ConsoleApp1\ConsoleApp1\Program.cs:line 3
```

### Pula wątków

Podczas uruchamiania wątku kilkaset mikrosekund jest poświęcone na przeprowadzenie operacji przygotowujących wątek (utworzenie stosu dla zmiennych lokalnych). **Pula wątków** pozwala na eliminację tego rodzaju kosztu, ponieważ do dyspozycji pozostaje pula już utworzonych i gotowych do użycia wątków. Dzięki czemu pozwala na wykonywanie krótkich operacji bez narzutu związanego z inicjalizacją.

Dobre praktyki podczas stosowania puli wątków:

- nie można definiować nazwy wątkowi, pochodzącemu z puli
- wątki pochodzące z puli są zawsze wątkami działającymi w tle
- blokujące wątki pochodzące z puli mogą doprowadzić do zmniejszenia wydajności

UWAGA: wątkowi z puli można zmienić priorytet, zostanie on przywrócony do domyślnego, gdy wątek wróci do puli.

Właściwość `Thread.IsThreadPoolThread` pozwala określić czy wątek pochodzi z puli.

Najłatwiejszym sposobem na uruchomienie zadania w wątku pochodzącym z puli jest użycie wywołania `Task.Run()`.

```csharp
Task.Run(() => Console.WriteLine("Witaj w puli wątków"));
```

Przeznaczeniem puli wątków jest również gwarancja, ze tymczasowy szczyt operacji wymagających mocy obliczeniowej nie spowoduje **nadsubskrypcji** procesora. Jest to sytuacja, gdy istnieje więcej aktywnych wątków niż rdzeni procesora, więc system operacyjny zmuszony jest to ograniczania czasu działania tych wątków. Prowadzi to do zmniejszenia wydajności, ponieważ wydzielanie wątkom czasu wymaga kosztownego przełączania kontekstu i może skutkować unieważnieniem bufora procesora.

Środowisko CLR unika nadsubskrypcji poprzez kolejkowanie zadań i ograniczenie ich uruchamiania. Na początku liczba uruchomionych zadań jest równa liczbie rdzeni, a następnie przeprowadzane jest dostrojenie procesu współbieżności za pomocą algorytmu **hill climbing** (jeśli przepustowość ulega poprawie, zmiana kontynyuowana jest w tym samym kierunku, jeśli się pogarsza, w przeciwnym), który nieustannie dostraja obciążenie.

Strategia ta sprawdza się najlepiej jeśli spełnione są następujące warunku:

- wykonywane operacje sa najczęściej krótkie (poniżej 250ms, idealnie poniżej 100ms), dzięki temu środowisko ma wystarczająco czasu na przeprowadzenie pomiaru i korektę
- pula nie może być zdominowana przez zadania, które przez większość czasu blokują wątek.

Blokowanie jest problematyczne, ponieważ dostarcza środowisku uruchomieniowemu CLR błędny obraz sytuacji dotyczącej obciążenia. Środowisko potrafi wykryć tego typu sytuację i zapobiegać im np. przez dodanie kolejnych wątków do puli, ale wtedy jednocześnie zwiększamy podatność na nadsubskrypcje. Zachowanie dobrej higieny w puli wątków jest bardzo ważne, szczególnie jeśli chcemy w pełni i wydajnie wykorzystać procesor.

## Zadania

Ograniczenia wątków:

- nie istnieje łatwy sposób otrzymania "wartości zwrotnej" z wątku, szczególnie jeśli użyliśmy wywołania `Join`. Konieczne jest przygotowanie współdzielonego elementu składowego, to samo tyczy się wyjątków, które są trudne do przechwycenia i propagowania.
- nie można nakazać wątkowi rozpoczęcia wykonywania innej operacji po zakończeniu pierwszej. Należy użyć wywołania `Join` co oznacza zablokowanie własnego wątku w tym czasie.
- użycie wątków powoduje implikacje w zakresie wydajności, w przypadku potrzeby przeprowadzenia wielu współbieżnych operacji związanych z I/O oznacza zużycie setek MB wyłącznie na obciążenie wynikające ze stosowania wątków.

Powoduje to trudności w przygotowaniu dużych operacji współbieżnych poprzez połączenie mniejszych i wymusza zależność od ręcznej synchronizacji i nakładania blokad co powoduje powstanie problemów m. in. z zakleszczeniem.

Rozwiązaniem na wyżej wymienione problemy jest klasa `Task`, która została wspomniana w sekcji "Pula wątków". Stanowi abstrakcje wyższego poziomu, która imituj współbieżną operację, która może, ale nie musi być wykonywana w oddzielnym wątku. Główną zaletą zadań jest możliwość ich **kompozycji** (można je łączyć) poprzez zastosowanie mechanizmu **kontynuacji**. Zadania korzystają z puli wątków co znacząco zmniejsza początkowe opóźnienie. Dzięki użyciu `TaskCompletionSource` zadania mogą wykorzystać oparte na wywołaniach zwrotnych podejścia, które pozwala uniknąć wątków przy operacjach związanych z I/O.

UWAGA: Należy pamiętać, że zadania domyślnie używają puli wątków, które są wątkami działającymi w tle.

Metoda `Task.Run` tworzy "gorące" zadania (uruchamiane zaraz po utworzeniu). Można tworzyć również zadania "zimne" - `new Task(() => Console.WriteLine("..."))` jednak jest to rzadko spotykane.

Status zadania można monitorować za pomocą włąsciwości `Status`. Jeśli natomiast chcemy zaczekać na zakończenie zadania należy wywołać metodę `Wait()`. Jest to odpowiednik wywołania `Join()` na wątku.

Domyślnie środowisko CLR uruchamia zadania za pomocą puli wątków co jest idealnym rozwiązaniem dla krótko działających zadań. Natomiast dla dłużej wykonywanych blokujących operacji można wymusić, aby zadanie nie pochodziło z puli wątków. Wykonywanie jednego czasochłonnego zadania przy użyciu puli wątków nie powoduje problemu, natomiast większa ilość może prowadzić do zmniejszenia wydajności, przy czym przy tego typu operacjach zwykle istnieje lepsze rozwiązanie niż `TaskCreationOptions.LongRunning`.

Sposoby radzenia sobie z długimi zadaniami:

- jeśli zadania są związane z I/O wtedy `TaskCompletionSource` i funkcje asynchroniczne pozwalają na implementację współbieżności za pomocą wywołań zwrotnych
- jeśli zadania są związane z obliczeniami, wtedy można zastosować wzorzec kolejka producent-konsument, który zdławi współbieżność dla tych zadań i pozwoli uniknąć negatywnego wpływu tych zadań.

```csharp
Task t = Task.Factory.StartNew(
    () => Console.WriteLine("Long running process..."),
    TaskCreationOptions.LongRunning);
```

### Wartości zwrotne i obsługa wyjątków

Klasa `Task` zawiera ogólnego przeznaczenia impelemntację `Task<TResult>`, która pozwala zadaniu na wyemitowanie wartości zwrotnej. Wynik może być pobrany po zakończenie zadania przy pomocy właściwości `Result`. Dostęp do tej właściwości spowoduje zablokowanie bieżącego wątku aż do chwili zakończenia zadania.

```csharp
Task<int> t = Task.Run(() => 42);

Console.WriteLine(t.Result);
```

W przeciwieństwie do wątków obsługa wyjątków jest znacznie prostsza. Zadania w wygodny sposób propagują wyjątki automatycznie do komponentu, który wywoła `Wait()` lub `Result` egzemplarza.

Środowisko uruchomieniowe CLR opakowuje wyjątek w `AggregateException`, ze względu na łatwiejszą obsługę w przypadku zasotoswania programowania równoległego.

Istnieje możliwość sprawdzenia poprawności zadania przy pomocy właściwości `IsFaulted` i `IsCanceled` bez konieczności ponownego zgłaszania wyjątku. Jeśli obie własciwości zwrócą `false` to znaczy, że zadanie zakończyło się bez błędu.

Dla zadań typu "fire&forget" (zdefiniuj i zapomnij) dobrą praktyką jest wyraźna obsługa wyjątku, aby uniknąć niebezpieczeństwa cichej awarii, jak to ma miejsce w przypadku wątku. Wyjątki nieobsłużone nazywami **niezaobserwowanymi**.

Na poziomie globalnym za pomocą statycznego zdarzenia `TaskScheduler.UnobservedTaskException` istnieje możliwość subskrypcji niezaobserwowanych wyjątków.

### Kontynuacja

Kontynuacja jest to mechanizm mówiący - "Po zakończeniu kontynuuj pracę robiąc coś innego". Jest zwykle implementowana przez wywołanie zwrotne, wykonywane po ukończeniu danej operacji. Istnieją dwa sposoby definiowania kontynuacji: `GetAwaiter()` i `OnCompleted()` oraz `Task.ContinueWith()`.

- Awaiter jest szczególnie użyteczny i wykorzystywany w funkcjach asynchronicznych C#

Wywołąnie `GetAwaiter()` powoduje zwrot obiektu awaitera, którego metoda nakazuje poprzedniemu zadaniu `t` uruchomienie delegatu po zakończeniu zadania lub jego niepowodzeniu. Kontynuacja na zakończonym zadaniu będzie przeprowadzona natychmiast.

W przypadku niepowodzenia zadania wyjątek zostanie ponownie zgłoszony w kontynuacji gdy kod wywoła metodę `GetResult()`. Zaletą wywołania `GetResult()` w stosunku do `t.Result` jest zgłoszenie wyjątku bez opakowania w `AggregateException`. Dla zadań o typie zwrotnym `void` metoda `GetResult()` służy jedynie do ponownego zgłoszenia wyjątku.

```csharp
Task<int> t = Task.Run(() => CalculatePrimeNumbersCount(1000));

TaskAwaiter<int> awaiter = t.GetAwaiter();
awaiter.OnCompleted(() =>
{
    int result = awaiter.GetResult();
    Console.WriteLine(result);
});

int CalculatePrimeNumbersCount(int number)
{
    // Całkowita ilość liczb pierwszych w zbiorze 0...number
}
```

Jeżeli obecny jest kontekst synchronizacji metoda `OnCompleted()` automatycznie go przechwytuje i przekazuje kontynuację do tego kontekstu. Jest to niezwykle użyteczne w bogatych aplikacjach klientów, gdzie przekazujemy kontynuację spowrotem do wątku interfejsu użytkownika. Jednak może być niepożądane w przypadku tworzenia bibliotek, ponieważ przekazywanie kontekstu jest kosztowne i powinno następować jedynie po opuszczeniu kodu biblioteki, a nie pomiędzy wywołaniami metod. Rozwiązaniem jest użycie `ConfigureAwait(false)`.

Jeżeli nie istnieje żaden kontekst synchronizacji lub użyty został `ConfigureAwait(false)`to wówczas kontynuacja będzie wykonywana w tym samym wątku co poprzednie zadanie co pozwala na uniknięcie niepotrzebnego obciążenia przy przełączaniu.

- Metoda `ContinueWith()`

Wartością zwrotną metody jest egzemplarz `Task`, który jest użyteczny podczas kolejnych kontynuacji. Wykorzystanie tego mechanizmu wiąże się z pracą z `AggregateException` i ręczną obsługą marshallingu. W kontekście innym niż kontekst interfejsu użytkwonika konieczne jest podanie `TaskContinuationOptions.ExecuteSynchronously`, jeśli praca ma być kontynuowana w tym samym wątku, w przeciwnym wypadku wątek wróci do puli. `ContinueWith()` ma zastosowanie w scenariuszach programowania równoległego.

```csharp
Task<int> t = Task.Run(() => CalculatePrimeNumbersCount(1000));

Task nextTask = t.ContinueWith(r =>
{
    int result = r.Result;
    Console.WriteLine(result);
});

int CalculatePrimeNumbersCount(int number)
{
    // Całkowita ilość liczb pierwszych w zbiorze 0...number
}
```

### Klasa `TaskCompletionSource`

Innym sposobem na utworzenie zadania jest wykorzystanie klasy `TaskCompletionSource`, która pozwala utworzyć zadanie dla dowolnej operacji rozpoczynającej i kończącej się w przyszłości. Działanie polega na przydzieleniu zadania podległego ("slave"), którym można ręcznie sterować przez wskazanie kiedy operacja się kończy lub ulega awarii.

Tego typu podejście jest idealne dla operacji związanych z I/O, ponieważ zyskujemy wszystkie zalety zadań (wartości zwrotne, wyjątki, kontynuacje) bez blokowania wątku podczas przeprowadzania operacji. Klasa `TaskCompletionSource` udostępnia właściwość `Task`, na którego wykonanie możemy czekać i do którego możemy dołączyć kontynuację. Jedyną różnicą jest to, że to zadanie jest całkowicie kontrolowane przez klasę `TaskCompletionSource` za pomocą metod `SetResult()` oraz `SetException()`.

Wywołanie powyższych metod sugeruje, że zadanie zostało zakończone i oczekuje się, że każda z nich zostanie wywołana jednokrotnie. W przypadku ponownego wywołania nastąpi zgłoszenie wyjątku. W tym celu utworzono również metody `Try*()`, które po prostu zwrócą `false`.

Klasa `TaskCompletionSource` pozwala na utworzenie zadania, które nie jest powiązane z żadnym delegatem. To daje całkowita kontrolę nad tym kiedy i jak ma się zadanie zakończyć.

Zastosowanie:

- Użycie Task-based API, które muszę być tworzone w oparciu o wzorzec "event-based asynchronous" lub "callback-based".
- Opakowanie sterego kodu asynchronicznego, który korzysta z callback'ów.
- Tworzenie zadań, które nie są niepowiązane z wątkami

UWAGA: `TaskCompletionSource` może być częściej używany w starych wersjach .NET, ponieważ w obecnych może być on śmiało zastąpiony `Task.FromResult<T>()`

## Reguły i funkcje asynchroniczne

**Operacja synchroniczna** - wykonuje swoją pracę przed przekazaniem kontroli z powrotem do komponentu, który ją wywołał.

**Operacja asynchroniczna** - wykonuje większość pracy po przekazaniu kontroli z powrotem do komponentu, który ją wywołał

Metoda asynchroniczna inicjuje i zwykle szybko zwraca kontrolę nad przebiegiem działania programu i dlatego jest również określana mianem **metody nieblokującej**:

- `Thread.Start()`
- `Task.Run()`
- Metody dołączające do zadań kontynuację

Zasada w programowaniu asynchronicznym polega na tym, że długo wykonywane funkcje są tworzone w sposób asynchroniczny. Jest to odmienne podejście w stosunku do konwencjonalnego polegającego na wywoływaniu ich w nowym wątku lub zadaniu w celu zastosowania współbieżności.

Różnica w podejściu asynchronicznym polega na tym, że współbieżność jest inicjowana **wewnątrz** długo wykonującej się funkcji, a nie na zewnątrz. Wiąże się to z następującymi korzyściami:

- współbieżność związana z wejściem/wyjściem może być zaimplementowana bez wykorzystania wątków (`Task.FromResult<T>()`, `Task.CompletedTask`) co oznacza poprawę skalowalności i efektywności
- bogate aplikacje klienta zawierają mniejszą ilość kodu w wątkach roboczych, co oznacza uproszczenie sposobu zapewnienia bezpieczeństwa wątków.
- refaktoryzacja kodu jest prostsza ze względu na mniejszy narzut kodu związanego z zarządzaniem wątkami

W przypadku asynchronicznego drzewa wywołań nie ma potrzeby uruchamiania nowego wątku aż do momentu gdy naprawdę stanie się konieczny (z reguły dość późno).

Rozważmy następujący przykład obliczający liczby pierwsze:

```csharp
int GetPrimesCount(int start, int count)
{
    return ParallelEnumerable.Range(start, count)
        .Count(n => Enumerable
            .Range(2, (int)Math.Sqrt(n) - 1)
            .All(i => n % i > 0));
}

void DisplayPrimesCount()
{
    for (int i = 0; i < 10; ++i)
        Console.WriteLine(GetPrimesCount(i * 1000 + 1, (i + 1) * 1000));
}
```

Współbieżność gruboziarnista - współbieżność inicjalizowana jest na początku drzewa wywołań.

Dla powyższego przykładu inicjalizacja współbieżności gruboziarnistej odbywa się w następujący sposób. Zaalokowane zostało nowe zadanie z puli od samego początku wywołania metody `DisplayPrimesCount`

```csharp
Task.Run(() => DisplayPrimesCount());
```

Współbieżność drobnoziarnista (fine-grained concurrency) - Współbieżność inicjalizowana jest nisko w drzewie wywołań Należy jednak pamiętać, że większa drobnoziarnistość pociąga za sobą większe narzuty na tworzenie i obsługę tych równoległych wątków.

W przypadku współbieżności drobnoziarnistej należałoby skorzystać z kontynuacji i klasy `TaskCompletionSource`. Na szczęście dzięki wsparciu ze strony języka cała ta praca konfiguracyjna jest wykonywana przez CLR przy wykorzystaniu słów kluczowych `async` i `await`.

```csharp
async Task DisplayPrimesCountAsync()
{
    for (int i = 0; i < 10; ++i)
    {
        int result = await GetPrimesCountAsync(i * 1000 + 1, (i + 1) * 1000);
        Console.WriteLine(result);
    }
}

Task<int> GetPrimesCountAsync(int start, int count)
{
    return Task.Run(() =>
        ParallelEnumerable.Range(start, count)
            .Count(n => Enumerable
                .Range(2, (int)Math.Sqrt(n) - 1)
                .All(i => n % i > 0)));
}
```

Jak widać zaalokowanie wątku nastąpiło niżej w drzewie wywołań metody, a wywołanie metody `await DisplayPrimesCountAsync()` będzie oczekiwać na zakończenie wywołania.

Słowa kluczowe `async` i `await` pozwalają na tworzenie kodu asynchronicznego, które będzie miał tę samą strukturę i prostotę co kod synchroniczny. Poniżej przedstawiono zasadę działania operatora `await`:

```csharp
var result = await wyrażenie;
// polecenia;
```

Jest przez kompilator rozszerzany na postać:

```csharp
var awaiter = wyrażenie.GetAwaiter();
awaiter.OnCompleted(() =>
{
    var result = awaiter.GetResult();
    //polecenia;
})
```

Modyfikator `async` został wprowadzony, aby nakazać kompilatorowi potraktowanie `await` jako słowa kluczowego, co gwarantuje kompatybilność wsteczną. Jest on podobny do modyfikatora `unsafe`, który nie ma wpływu na sygnaturę metody i jej metadane, ale wywiera wpływ na zdarzeniach, które zajdą wewnątrz tej metody.

Działanie async/await polega na tym, że po napotkaniu wyrażenia `await` kontrola nad przebiegiem programu wraca do komponentu wywołującego (podobnie jak `yield return`), jednak wcześniej CLR dołącza do oczekiwanego zadania kontynuacje. W ten sposób mamy pewność, że po jego zakończeniu nastąpi przejście z powrotem do metody i kontynuacja działania będzie od miejsca w której została przerwana, a wartość zwrotna zostanie przypisana wyrażeniu `await`.
Kompilator opiera się na kontynuacji w celu wznowienia działania po wyrażeniu `await`. Wykonanie może, ale nie musi być wznowione w tym samym wątku, w którym została wywołana metoda, może być wznowione w wątku, w którym zakończyło się zadanie w zależności czy został użyty kontekst synchronizacji (zwykle nie ma to znaczenia chyba, że użyto specyficznych mechanizmów dla wątków - lokalny magazyn danych dla wątku)

Można to porównać do podróży taksówką po mieście. W przypadku użycia kontekstu synchronizacji zawsze będziemy podróżować tą samą taksówką. W przypadku jego braku za każdym razem możemy mieć inną. Niezależnie od tego podróż zawsze odbywa się w taki sam sposób.

UWAGA: Wyrażenie `await` nie może być używane wewnątrz `lock`. Wynika to z podatności na zakleszczenia. W przypadku `await` niekoniecznie musimy wrócić do tego samego wątku co może doprowadzić do tego, żę `lock` nigdy nie zostanie zdjęty.

## Przerwanie operacji

W celu uzyskania tokena przerwania operacji należy zacząć od inicjalizacji `CancellationTokenSource`. Ten egzemplarz udostępnia właściwość `Token`, która zwraca `CancellationToken`.

W celu przerwania operacji należy wywołąć metodę `Cancel()`. Dodatkowo struktura `CancellationToken` dostarcza metodę `Register()`, która pozwala na zarejestrowanie delegatu wywołania zwrotnego, który zostanie wywołany po zainicjowaniu przerwania operacji.

## Wzorzec asynchroniczny oparty na zadaniu (Task-based asynchronous pattern - TAP)

Podstawowe cechy metody TAP:

- Zwraca "gorące" (czyli dziąłające) zadanie w postaci egzemplarza `Task` lub `Task<Result>`
- Ma przyrostek "Async"
- Jest przeciążona w celu akceptacji tokena przerwania i/lub interfejsu `IProgress<T>`
- Szybko przekazuje kontrolę z powrotem do komponentu wywołującego (ma jedynie krótką początkową fazę synchroniczną)
- Nie wykorzystuje wątku jeśli operacja jest związana z wejściem-wyjściem

## Łączniki zadań

Metoda `Task.WhenAny()` zwraca zadanie, które będzie ukończone za zakończeniu jednego ze zbioru zadań. Jeśli inne zadanie niż wygrywające ulegnie awarii zgłoszony wyjątek pozostanie niezauważony, chyba, żę oczekujemy na zakońćzenie zadania lub sprawdzamy jego właściwość `Exception`.

Wywołanie `WhenAny` jest użyteczne podczas stosowania zegarów lub funkcji przerwania operacji, które w przeciwnym razie nie są obsługiwane.

```csharp
Task<string> task = DoSomethingAsync();

Task winner = await Task.WhenAny(task, Task.Delay(500));
if (winner != task)
    throw new TimeoutException();
string result = await task;

Task<string> DoSomethingAsync()
{
    throw new NotImplementedException();
}
```

Uwaga: Należy pamiętać, że w przypadku wywołania `WhenAny` z zadaniami o różnych typach zwycięzca zostanie podany jako zwykły `Task`.

Metoda `Task.WhenAll()` zwraca zadanie, które zostanie ukończone po ukończeniu **wszystkich** zadań przeznaczonych do wykonania. W przypadku wywołania `await t1; await t2; ...` w przypadku awarii w `t1` system nie będzie oczekiwać na zakończenie kolejnych zadań, a wyjątki pozostaną niezauażone. W przypadku `Task.WhenAll` zadanie nigdy nie bedzie uznane za zakończone jeśli nie zakończą się wszystkie zadania (niezależnie od ich powodzenia). W przypadku wielu niepowodzeń zostanie zgłoszony wyjątek `AggregateException`, który zagreguje błędny ze wszystkich zadań. Rezultatem wywołania `Task.WhenAll` z zadaniami `Tast<TResult>` jest tablica wyników ze wszystkich zadań.

Można też pisać własne łączniki zadań. Przykładem może być "porzucenie" zadania, po wystąpieniu `CancellationToken` jeśli to zadanie natywnie nie wspiera przerwania:

```csharp
internal static class TaskExtensions
{
    public static Task<TResult> WithCancellation<TResult>(this Task<TResult> task, CancellationToken token)
    {
        var tcs = new TaskCompletionSource<TResult>();
        var reg = token.Register(() => tcs.TrySetCanceled());

        task.ContinueWith(t =>
        {
            reg.Dispose();
            if (t.IsCanceled)
                tcs.TrySetCanceled();
            else if (t.IsFaulted)
                tcs.TrySetException(t.Exception);
            else
                tcs.SetResult(t.Result);
        });

        return tcs.Task;
    }
}
```

Uwaga: Istnieje metoda `Task.WaitAsync`, która spełnia powyższą funkcjonalność.

## Nakładanie blokad i zapewnienie bezpieczeństwa wątków

Blokady pozwalają rozwiązać problem związany bezpieczeństwem wątków, ale jednocześnie mogą powodować własne problemy jak np. zakleszczenie.

**Synchronizacją** nazywamy działanie polegające na koordynowaniu jednocześnie wykonywanych czynności tak aby otrzymać przewidywalny wynik. Działania te są szczególnie ważne w przypadku gdy wiele wątków korzysta z jednego zbioru danych. Do najprostszych i najbardziej przydatnych zalicza się techniki kontynuacji i kombinatory zadań.

Program i metoda są bezpieczne ze względu na wątki jeśli działają prawidłowo w każdym środowisku wielowątkowym. Bezpieczeństwo wątkowe osiąga się przede wszystkim dzięki eliminowaniu możliwości interakcji pomiędzy wątkami.

Typy ogólnego przeznaczenia nieczęsto charakteryzują sie bezpieczeństwem wątkowym w całości z kilku powodów:

- opracowanie w pełni bezpiecznego ze względu na wątki typu jest (wiele pól, interakcja)
- mechanizmy zapewniające bezpieczeństwo wątkowe mogą negatywnie wpływać na wydajność
- typ bezpieczny ze względu na wątki niekoniecznie sprawia, że cały program jest bezpieczny wątkowo

Z tych powodów mechanizmy bezpieczeństwa wątkowego implementuje się z reguły tylko wtedy gdy jest to konieczne w konkretnej sytuacji.

Jedną z automatycznych technik jaką można wykorzystac przy implementacji bezpieczeństwa wątkowego jest automatyczne blokowanie przy użyciu klasy bazowej `ContextBoundObject` i dodaniu atrybutu `Synchronization`. Wywołanie metody lub własności na obiekcie tej klasy spowoduje założenie blokady na cały obiekt. Jest to przydatne, ale jednocześnie niebezpieczne ze względu na dużą część logiki, która dzieje się za naszymi plecami.

Uwaga: Dlaczego enumerator i typ przeliczalny są od siebie odzielne? Dzieje się tak ze względu na bezpieczeństwo wątkowe. Dwa wątki mogą przeglądać kolekcję, ponieważ każdny z nich otrzymuje osobny obiekt enumeratora

### Blokady wykluczające

Konstrukcje blokowania wykluczającego pozwalają na wykonywanie działań tylko jednemu wątkowi lub wykonywanie tylko jednej sekcji kodu na raz. Ich głównym zadaniem jest zapewnienie wątkom niezakłóconej możliwości zapisu we wspólnych kontstrukcjach reprezentujących stan obiektów. Do wykluczających blokad zaliczają się konstrukcje:

- `lock` - najczęściej stosowana i najwygodniejsza w użyciu (opakowana klasa `Monitor`)
- `Mutex` - umożliwia objęcie blokadą kilku procesów (blokady mogą obejmować cały komputer)
- `SpinLock` - implementuje mikrooptymalizację redukującą liczbę przełączeń kontekstu w programach o wysokim współczynniku współbieżności

#### Instrukcja `lock`

Na instrukcję `lock` składa się obiekt synchronizacji (najczęściej jakieś pole), który może być blokowany tylko przez jeden wątek na raz i gdy to się stanie wszystkie pozostałe wątki potrzebujące dostępu do blokowanego zasobu muszą czekać na zwolnienei tej blokady. Jeśli o blokadę zabiega więcej niż jeden wątek, wątki są ustawiane w "kolejce gotowości" i otrzymują prawo do jej założenia zgodnie z kolejnością zgłoszeń (zwykle, czasem nie ze względu na system Windows i CLR). Mówi się, że blokady wykluczające zapewniają **szeregowy** dostęp do chronionych zasobów.

W jezyku C# instrukcja `lock` to w zasadzie składniowy skrót do wywołania metody `Monitor.Enter` i `Monitor.Exit` z blokiem try-catch

```csharp
object _locker = new object();

lock(_locker)
{
    // Some Actions
}
```

```csharp
bool lockTaken = false;
try
{
    Monitor.Enter(_locker, ref lockTaken);
    // Some Actions
}
finally
{
    if(lockTaken)
        Monitor.Exit(_locker);
}
```

Uwaga: Wywołanie metody `Monitor.Exit`, bez wcześniejszego `Enter` spowoduje zgłoszenie wyjątku.

Uwaga: Wywołanie zwykłej metody `Monitor.Enter` może spowodować subtelny błąd wynikający z tego, ze pomiędzy metodą `Enter`, a kolejną linią może wystąpić wyjątek (koniec pamięci, abort), który spowodowałby wyciek blokady - nigdy nie mogłaby być zdjęta. Dlatego wprowadzono metodę przeciążoną. Dokładnie taki kod jak wyżej jest generowany przez kompilator.

W klasie `Monitor` istnieje też metoda `TryEnter`, która pozwala na określenie limitu czasu na założenie blokady, jeśli uda się założyć zwraca `true`. Dodatkowo istnieje też bezparametrowa wersja, która umożliwia sprawdzenie stanu blokady.

Uwaga: Wymaganiem stawianym przed obiektem synchronizacji jest to, że musi on być typu **referencyjnego**

Kiedy stosować blokadę? Podstawowa zasada jest taka, że należy blokować każde wspólne pole z możliwością zapisu. Możliwość tą należy rozpatrywać w najprostszych nawet przypadkach jak inkrementacja.

Blokady powodują powstanie **bariery pamięci**, które jest "ogrodzeniem" w obrębie, którego nie można zmieniać kolejności operacji, ani stosować buforowania.

Jeśli grupa zmiennych jest zawsze odczytywana i zapisywana w obrębie tej samej blokady to można powiedzieć, że zmienne są odczytywane i zapisywane atomowo. Należy mieć na uwadze, żę blokady nie zawsze zapewniają atomowość, gdy w ich obrębie dojdzie do wystąpienia wyjątku.

Uwaga: Wątek może wielokrotnie zablokować jeden obiekt przy pomocy blokad zagnieżdżonych (wielowejściowych). W takich przypadkach zwolenienie blokady następuje dopiero po zakończeniu pierwszej blokady od zewnątrz (lub wykonania odpowiedniej liczby wywołań `Monitor.Exit`)

**Zakleszczenie** - sytuacja, w której dwa wątki czekają na zasób blokowany przez drugi wątek przez co żaden z nich nie może kontynuować parcy. Zakleszczenia zaliczają się do najtrudniejszych problemów wielowątkowości, zwłaszcza gdy jest wiele powiązanych obiektów. Ironia polega na tym, że żeby uniknąć zakleszczeń najlepiej stosować jak najmniej blokad oraz stosować inne mechanizmy synchronizacji jak kontynuacja oraz dbać o niezależność danych.

#### Klasa `Mutex`

Klasa `Mutex` przypomina instrukcję `lock` z tą różnicą, że może obejmować wiele procesów jak i również cały komputer/aplikację. Wydajnościowo jest to gorsze rozwiązanie niż instrukcja `lock` (różnica ~20 razy). Korzystając z klasy `Mutex` do blokowania należy wywołać metodę `WaitOne`, a żeby zdjąć blokadę wywołujemy `ReleaseMutex`. Podobnie jak w przypadku instrukcji `lock` blokada może być zniesiona tylko przez wątek, który ją założył.

Uwaga: Aby mutex był dostępny w całym komputerze, należy mu nadać nazwę, dzięki temu każdy będzie mógł się do niego odwołać.

```csharp
Mutex mutex = new Mutex(false);
try
{
    mutex.WaitOne();
    // Some Actions
}
finally
{
    mutex.ReleaseMutex();
}
```

### Blokowanie bez wykluczenia

Blokowanie bez wykluczenia umożliwia ograniczenie współbieżności. Do konstrukcji z tej grupy zaliczają się `Semaphore(Slim)`, `ReaderWriterLock(Slim)`

#### Semafory - `Semaphore`

Semafor jest jak klub nocny - ma określoną pojemność, której pilnuje bramkarz. Kiedy lokal się zapełni, nowi goście nie są wpuszczani i na zewnątrz ustawia się kolejka. Od tej pory za każdą osobę opuszczającą klub może wejść jedna nowa osoba. Konstruktor otrzymuje informację o aktualnej liczbie dostępnych miejsc i całkowitej pojemności.

Uwaga: Semafor, który ma jedno miejsce jest funkcjonalnie podobny do blokady z tą różnicą, że nie ma właściciela i każdy może go wywołać - **jest obojętny na wątki**

Dzięki semaforom można ograniczyć intensywność współbieżności, tzn. uniknąć uruchomienia na raz zbyt wielu wątków wykonujących blok kodu.

Uwaga: Jeśli semafor dostanie nazwę może objemować wiele procesów.

```csharp
SemaphoreSlim sem = new SemaphoreSlim(3);

for (int i = 1; i <= 5; i++)
{
    int localId = i;
    Task.Run(() => Enter(localId));
}

Console.ReadLine();

void Enter(int id)
{
    Console.WriteLine($"{id} chce wejść");
    sem.Wait();

    Console.WriteLine($"{id} jest w środku");
    Thread.Sleep(1000 * id);
    Console.WriteLine($"{id} wychodzi");
    sem.Release();
}

//Output:
// 1 chce wejść
// 2 chce wejść
// 2 jest w środku
// 4 chce wejść
// 4 jest w środku
// 1 jest w środku
// 3 chce wejść
// 5 chce wejść
// 1 wychodzi
// 3 jest w środku
// 2 wychodzi
// 5 jest w środku
// 4 wychodzi
// 3 wychodzi
// 5 wychodzi
```

#### Blokady odczytu i zapisu - `ReaderWriterLock`

Często się zdarza, że egzemplarze obiektu są bezpieczne wątkowo dla operacji odczytu, ale nie są dla operacji zapisu. Może też być tak, że w większości wykonywane są operacje odczytu, a zapis jest sporadyczny. W tym celu sprawdzi się klasa `ReaderWriterLock`, jest podobna do standardowych metod wykluczających przy czym dzięki jej zastosowaniu możemy liczyć na mniejszą liczbę blokad.

Można posługiwać się dwoma podstawowymi rodzajami blokad - odczytu i zapisu:

- Blokada zapisu ma charakter uniwersalnie wykluczający
- Blokada odczytu jest zgodna z innymi blokadami odczytu

Zgodnie z tym wątek przetrzymujący blokadę zapisu uniemożliwia wszystkim innym wątkom możliwość dostępu do blokady **odczytu i zapisu**, ale jeśli żaden wątek nie przetrzymuje dostępu do blokady zapisu to do blokady odczytu może współbieżnie uzyskać dostęp dowolna liczba wątków.

Klasa `ReaderWriterLock` umożliwia jednoczesne uruchomienie większej liczby operacji odczytu niż zwykła blokada. Można to sprawdzić poprzez dostęp do właściwości `CurrentReadCount`.

W rozważanej blokadzie istnieje dodatkowa funkcjonalność, która nosi nazwę **blokady z możliwością uaktualnienia**, która umożliwia zmianę blokadu odczytu na blokadę zapisu. Ma to szczególne zastosowanie w przypadku gdzie chcielibyśmy najpierw odczytać kolekcję w poszukiwaniu elementu, który chcemy dodać, a następnie go dodać. Metoda ta to `EnterUpgradeableReadLock`. Należy pamiętać tylko, że w przypadku blokad z możliwością uaktualnienia może istnieć tylko jedna, w przeciwieństwie do wielu blokad odczytu.

```csharp
List<int> numbers = new List<int>();

ReaderWriterLockSlim rw = new ReaderWriterLockSlim();
while(true)
{
    int n = new Random().Next();
    rw.EnterUpgradeableReadLock();
    if(!numbers.Contains(n))
    {
        rw.EnterWriteLock();
        numbers.Add(n);
        rw.ExitWriteLock();
    }
    rw.ExitUpgradeableReadLock();
}
```

Uwaga: Możemy stosować blokady rekurencyjne tylko jeśli obiekt zostanie utworzony z parameter `LockRecursionPolicy.SupportsRecursion`. Należy tylko pamiętac, że kolejne blokady muszą być "mniejsze" - blokada odczytu -> blokada odczytu z możliwością uaktualnienia -> blokada zapisu

### Sygnalizacja przy użyciu uchwytów zdarzeń oczekiwania

**Sygnalizowanie** - Konstrukcje z tej grupy umożliwiają zablokowaniu wątku w oczekiwaniu na jedno lub więcej powiadomień od innych wątków. Do konstrukcji sygnalizujących zalcizają się `ManualResetEvent(Slim)`, `AutoResetEvent`, `CountdownEvent`, `Barrier`. Trzy pierwsze określa się wspólnym mianem "uchwytów oczekiwania na zdarzenia"

#### Klasa `AutoResetEvent`

Obiekt klasy `AutoResetEvent` jest jak bramka na żetony - włożenie jednego żetonu uprawnia do przejścia dalej jedną osobę. Słowo _Auto_ w nazwie klasy odnosi się do tego, że otwarta bramka automatycznie się zamyka, gdy ktoś przez nią przejdzie. Wątek czeka, lub zostaje zablokowany, przy bramce przez wywołanie metody `WaitOne`, a włożenie żetonu odbywa się przy pomocy metody `Set`. Żeton może być dostarczony przez każdy wątek. Obiekt można utworzyć podając w konstruktorze argument `bool`, który definiuje czy bramka jest domyślnie po utworzeniu otwarta.

```csharp
EventWaitHandle waitHandle = new AutoResetEvent(false);

Task.Run(() => Waiter());
await Task.Delay(3000);

waitHandle.Set();

void Waiter()
{
    Console.WriteLine("Czekanie...");
    waitHandle.WaitOne();
    Console.WriteLine("Jest sygnał.");
}

//Output:
//Czekanie... (pauza 3 sekundy)
//Jest sygnał.
```

Uwaga: Jeśli metoda `Set` zostanie wywołana, gdy nie oczekuje żaden wątek to uchwyt zostanie otwarty do momentu wywołania `WaitOne`. Przy czym przekazanie wielu "żetonów" nie spowoduje szerszego otwarcia bramki, przejdzie tylko jeden, a reszta się zmarnuje.

Metoda `Reset` powoduje zamknięcie bramki (jeśli jest otwarta).

Metoda `WaitOne` może przyjść opcjonalnie parametr limitu czasu, po upływie, którego zostanie zwrócona wartoć `false`. Wywołanie metody z zerowm czasem oczekiwania umożliwia sprawdzenie czy bramka jest otwarta.

Przy użyciu klasy `AutoResetEvent` można definiować sygnały dwustronne. Przykładem może być chęć wysłania przez wątek główny do wątku roboczego trzech sygnałów z rzędu. Gdyby operacja została wykonana jeden po drugim wywołując metodę `Set`, któryś z sygnałów mógłby się zgubić, ponieważ wątek roboczy mógłby nie zdążyć na czas przetworzyć żądania. Można to rozwiązać przy użyciu dwóch instancji klasy `AutoResetEvent`:

```csharp
EventWaitHandle ready = new AutoResetEvent(false);
EventWaitHandle go = new AutoResetEvent(false);

object locker = new object();
string message = null;

new Thread(Work).Start();

ready.WaitOne();
lock (locker)
    message = "test - 1";
go.Set();

ready.WaitOne();
lock (locker)
    message = "test - 2";
go.Set();

ready.WaitOne();
lock (locker)
    message = null;
go.Set();

void Work(object? obj)
{
    while (true)
    {
        ready.Set();
        go.WaitOne();
        lock(locker)
        {
            if (message != null)
                Console.WriteLine(message);
        }
    }
}
```

#### Klasa `ManualResetEvent`

Klasa `ManualResetEvent` działa jak prosta brama. Wywołanie metody `Set` powoduje otwarcie bramy i przepuszczenie **dowolnej** liczby wątków, któe wywołają `WaitOne`. Metoda `Reset` zamyka bramę. Przy następnym odblokowaniu bramy zostanę wpuszczone na raz wszystkie oczekujące wątki. Głównym zastosowaniem klasy `ManualResetEvent` jest sytuacja, w której jeden wątek ma odblokować wiele oczekujących na raz.

```csharp
ManualResetEvent signal = new ManualResetEvent(false);

new Thread(() =>
{
    Console.WriteLine("Oczekiwanie na sygnał...");
    signal.WaitOne();
    Console.WriteLine("Otrzymano sygnał!");
}).Start();

Thread.Sleep(2000);
Console.WriteLine("Wysłano sygnał.");
signal.Set();

//Output:
// Oczekiwanie na sygnał...
// Wysłano sygnał.
// Otrzymano sygnał!
```

Uwaga: Istnieje również zoptymalizwona wersja `ManualResetEventSlim`, która może być nawet 50 razy szybsza.

#### Klasa `CountdownEvent`

Klasa `CountdownEvent` umożliwia czekanie na więcej niż jeden wątek. Ma efektywną w pełni zarządzaną implementację. W konstruktorze przekazujemy liczbę sygnałów na które chcemy poczekać. Wywołanie metody `Signal` powoduje zmniejszenie licznika o jeden, a blokada zostanie zwolniona kiedy licznik spadnie do 0.

```csharp
CountdownEvent countdown = new CountdownEvent(3);

Task.Run(async () => await Work(3));
Task.Run(async () => await Work(5));
Task.Run(async () => await Work(2));

countdown.Wait();
Console.WriteLine("Wszystkie wątki zakończyły działanie.");

async Task Work(int id)
{
    int wait = id * 1000;
    Console.WriteLine($"Oczekiwanie {wait} s...");
    await Task.Delay(wait);
    Console.WriteLine($"Zakończon czekanie na {id}.");
    countdown.Signal();
}

// Output:
// Oczekiwanie 5000 s...
// Oczekiwanie 3000 s...
// Oczekiwanie 2000 s...
// Zakończon czekanie na 2.
// Zakończon czekanie na 3.
// Zakończon czekanie na 5.
// Wszystkie wątki zakończyły działanie.
```

Wartość licznika klasy można zmienić przy użyciu metody `AddCount`, przy czym jest to możliwe tylko do momentu jeśli licznik nie osiągnął wartości - to znaczy, że nie można "odsygnalizować" obiektu. Istnieje też implementacja `TryAddCount`, który zachowuje się podobnie jak inne metody typu Try\*

#### Uchwyty oczekiwania i kontynuacje

Zamiast czekać na uchwyt oczekiwania (i blokować wątek) można do niego przywiązać "kontynuację" za pomocą metody `ThreadPool.RegisterWaitForSingleObject`, jako argument przyjmuje ona delegat, który jest wykonywany w chwili otrzymania sygnału przez uchwyt oczekiwania. Kiedy uchwyt oczekiwania otrzymuje sygnał (albo upłynie limit czasu), na wątku z puli zostaje uruchomiony delegat. Później należy zwolnić niezarządzany uchwyt za pomocą metody `Unregister`.

#### Klasa `Barrier`

Klasa `Barrier` implementuje barierę wątku wykonawczego za pomocą, której można doprowadzić do spotkania w czasie wielu wątków. Klasa ta jest bardzo szybka i efektywna, a jej konstrukcja bazuje na metodach `Wait` i `Pulse` oraz blokadach pętlowych.

Utworzenie klasy `Barrier` przy użyciu wartości 3 powoduje, że metoda `SignalAndWait` zakłada blokadę póki nie zostanie wywołana trzy razy. Następnie wszystko zaczyna się od początku. Dzięki temu każdy wątek nadąża za pozostałymi wątkami.

```csharp
Barrier barrier = new Barrier(3, b => Console.WriteLine());

new Thread(Work).Start();
new Thread(Work).Start();
new Thread(Work).Start();


void Work()
{
    for (int i = 0; i < 5; i++)
    {
        Console.Write(i);
        barrier.SignalAndWait();
    }
}

// Output:
// 000
// 111
// 222
// 333
// 444
```

Dodatkową funkcjonalnością klasy `Barrier` jest możliwość wykonania delegaty po każdej fazie. Delegat wykonywany jest po wywołaniu metody `SignalAndWait`, ale przed zablokowaniem wątków. Przy pomocy tego delegatu można np. dokonać kombinacji danych z poszczególnych wątków. Nie musimy wtedy martwić się wywłaszczeniem, ponieważ wszystkie wątki i tak są zablokowane podczas działania delegatu
