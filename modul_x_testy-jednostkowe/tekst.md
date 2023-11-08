# Testy Jednostkowe

## Wstęp

Czy pisanie testów jest łatwe? Zdecydowanie nie. Ale to jedna z kilku uniwersalnych programistycznych umiejętności, w którą warto zainwestować. Wymaga to dużo praktyki, czasu, nauki i wyciągania wniosków z błędów. Jednak na dłuższą metę: bardzo się opłaca. A dodatkowo może być naprawdę przyjemne! Po wskoczeniu na pewien level znajomości tematu – nie wyobrażamy sobie programowania bez nich.

## Co to są testy jednostkowe

Definicje testów jednostkowych można mnożyć. Ja na przestrzeni lat ukułem taką:

    Test jednostkowy to kod wykonujący inny kod w kontrolowanych warunkach w ramach jednego procesu w pamięci w celu weryfikacji (bez ingerencji programisty), że testowana logika działa w ściśle określony sposób.

I każde słowo ma w tej definicji znaczenie:

“Kod wykonujący inny kod” – rozdzielamy kod testów od kodu produkcyjnego; nie zapominamy jednak, że o każdy kod trzeba dbać.

“w kontrolowanych warunkach” – test daje taki sam rezultat niezależnie od środowiska / maszyny / setupu.

“w ramach jednego procesu w pamięci” – testy jednostkowe są szybkie, powtarzalne i odizolowane od wszystkich innych operacji.

“w celu weryfikacji” – testy COŚ sprawdzają i jawnie definiują ten aspekt

“(bez ingerencji programisty)” – testy działają automatycznie

“testowana logika działa w ściśle określony sposób” – czy to oznacza, że testy eliminują występowanie błędów?

## Rola testów jednostkowych

Wyżej napisałem, że testami można sobie zaszkodzić. Zatem: czy warto z nich korzystać?

Koszt NIEPISANIA testów jest za duży, by go zaakceptować. Szczególnie jeśli weźmiemy pod uwagę korzyści płynące z różnych ról, jakie testy jednostkowe odgrywają w naszych systemach.

### Weryfikacja działania bez uruchamiania systemu

Oczywiste, prawda? To przecież podstawowa funkcja testów i każdy jest jej świadom! Jednak za chwilę nawet tutaj natkniemy się na pewien plot twist.

Ciągłe ręczne sprawdzanie działania aplikacji to straszna strata czasu. Trzeba poczekać na odpalenie serwera, rozgrzanie systemu. Trzeba przeklikać się przez interfejs i badać rezultat.

Dobre testy mogą tutaj bardzo pomóc!

Dzięki automatycznej weryfikacji, że testowany kod spełnia pewne założenia, nie musimy uruchamiać całego systemu. Wystarczy włączyć testy i poczekać chwilę na ich wykonanie.

Zielony pasek w test runnerze oznacza jedną z dwóch rzeczy: albo nasz kod działa, jak powinien (hurra!), albo mamy źle napisane testy (buuu).

Innej możliwości nie ma.

Sposób działania testów jest dość banalny:

autor testu (programista) dostarcza dane wejściowe (input),
test wykonuje instrukcje i…
sprawdza, czy rezultat działań (output) jest zgodny z oczekiwaniami.

Z czasem, gdy wraz z nabywaniem praktyki testy będą miały coraz większą wartość, zaufanie do nich będzie rosło. Możemy nawet dotrzeć do poziomu, na którym wystarczy jedno uruchomienie “na wszelki wypadek”, po zakończeniu implementacji całego ficzera.

Zatem: czy testy eliminują powstawanie błędów?

To właśnie wspomniany plot twist… Otóż: niekoniecznie!

Zgodnie z ustaloną wcześniej definicją: testy to po prostu kod. Kod testujący i kod testowany są napisane przez takiego samego programistę (a najczęściej tę samą osobę).

Bardzo często wchodzimy w świat testów z oczekiwaniem, że testy wyeliminują bugi w systemie. Niestety jest to błędne założenie. A potem z rozczarowaniem stwierdzamy, że tak się nie dzieje, i zniechęceni zamykamy temat.

Już na początku przygody z testami trzeba zaakceptować smutny fakt:

    Testy weryfikują, że kod działa tak, jak chce tego programista, ale niekoniecznie tak jak powinien.

Owszem, testy MOGĄ zredukować liczbę błędów. Jednak traktujmy to jako miły efekt uboczny, bo do tego celu potrzebne jest spełnienie dwóch warunków.

Warunek pierwszy: programista musi umieć pisać dobre testy (bo, jak wspomnieliśmy wyżej, złe testy mogą być szkodliwe).

Warunek drugi: programista musi rozumieć specyfikację zgodnie z intencjami jej autorów. Specyfikację napisaną dokładnie, precyzyjnie i klarownie. Poprzedzoną porządną analizą wymagań i wykryciem przypadków brzegowych. A wiemy przecież – i ja i Ty – że to często nie jest normą.

Podsumowując: testy nie gwarantują wyeliminowania bugów. Ale mogą znacznie zredukować prawdopodobieństwo błędów regresji (czyli wprowadzanie usterek do działającego oprogramowania).

### Zabezpieczenie przed błędami regresji

Od samego początku swojej testowej drogi (2007 rok!) podchodziłem do testów z wielkim zaufaniem i traktowałem je jako zapewnienie, że po zmianach kod nadal działa, jak powinien. No bo przecież “pasek test runnera zaświecił się na zielono”. Niestety, nie zawsze tak było. Ale wina leżała oczywiście wyłącznie po mojej stronie.

Prawda jest taka, że świetne testy chronią przed bugami przy zmianach w kodzie. Ale nauka pisania testów trwa i wymaga praktyki. Jednak, gdy już nauczymy się pisać testy tam gdzie trzeba i tak jak trzeba, zmiany w istniejącym kodzie będą o wiele łatwiejsze. Wchodzimy w legacy code jak nóż w masło (a często kod staje się legacy już w momencie napisania, co nie?).

Na pewnym poziomie możemy liczyć na to, że testy (nie tylko jednostkowe) wychwycą niechciane konsekwencje zmian. Czyli modyfikujemy kodzik z dużą dozą prawdopodobieństwa, że wprowadzamy tylko świadome zmiany i unikamy efektów ubocznych.

Gdy popełnimy błąd – testy to wychwycą. Muszą jednak spełniać ten najważniejszy warunek – muszą być dobre (cokolwiek to w tym momencie znaczy). W przeciwnym wypadku doświadczenia będą zgoła odmienne.

Słabe lub niepotrzebne testy utrudniają utrzymanie i rozwój systemu.

A niespodziewane zmiany to nie tylko nasza własna ingerencja! Może to być również modyfikacja wykonana przez innego członka zespołu. Albo zmiana w środowisku. W bazie danych. W zewnętrznej bibliotece (po podbiciu wersji). Czy nawet w zewnętrznym systemie, z którym się komunikujemy.

W tych przypadkach ponownie wychodzimy poza obszar testów jednostkowych… ale to nic. Mechanizmy nauki pisania innych testów są podobne, zmienia się tylko kontekst.

### Szybsze debuggowanie

Jedna z moich ulubionych korzyści płynących z pisania testów to możliwość debugowania wybranego elementu systemu bez uruchamiania całości. Możemy podpiąć debugger w kontekście jednego konkretnego testu, skonstruowanego pod jeden konkretny scenariusz, i cieszyć się śledzeniem wykonywanej pod spodem logiki bez oczekiwania na “wstanie i rozgrzanie” całego środowiska.

To niesamowicie przyspiesza proces identyfikacji i poprawiania usterek!

Ale dodatkowo ciekawym efektem rozwoju umiejętności pisania testów jest też stopniowa redukcja czasu spędzanego w debuggerze.

    Nie lubisz debugować? Naucz się pisać porządne testy.

Zobaczysz, że w miarę postępów w nauce tworzenia testów, coraz rzadziej będziesz korzystać z debuggera. Dla mnie była to wielka niespodzianka, którą powitałem z ogromnym entuzjazmem.

### Testy jako dokumentacja

Tworzenie dokumentacji do kodu to często strata czasu. Powód tego jest bardzo prosty: taka dokumentacja nadaje się do czegokolwiek tylko w momencie jej pisania. Po kilku dniach, tygodniach, miesiącach ma się nijak do opisywanego kodu. Chyba, że ktoś faktycznie dba o jej uaktualnienie, ale… No właśnie, dbasz?

Mówi się, że najlepszą dokumentacją kodu powinien być sam kod. Self-documenting code. I jest to prawda! A testy mogą być w takim wypadku BARDZO pomocne!

Testy to kod – więc są czytelne dla programisty.
Testy są regularnie uruchamiane – więc są zsynchronizowane z testowaną logiką.
Mało tego: testy nieustannie ewoluują. Rozwijają się wraz z systemem oraz zespołem! Z czasem tworzą coraz bardziej precyzyjną i coraz lepszą narrację dla produkcyjnych instrukcji.

W tym miejscu pokuszę się nawet o stwierdzenie, że testy transformują “życzenie” klienta w prawdziwe “wymaganie”. Bo są wykonywalną dokumentacją.

    Testy zmieniają deisrement w requirement

“Desirement” to tylko “chcenie”. ŻYCZĘ SOBIE, by system działał jakoś… ale nie mam na to gwarancji (poza słowem programisty).

“Requirement” to faktyczne wymaganie, zapisane za pomocą testu. Jeśli failed tests będziemy traktować tak poważnie jak błędy kompilacji, to będą one czynnikiem eliminującym możliwość wdrożenia danego builda. A to już jest konkretna korzyść i gwarancja, że nie tego elementu nie pominiemy.

### Weryfikacja specyfikacji

Wspominaliśmy wcześniej, że testy mają szansę eliminować występowanie błędów wtedy, gdy specyfikacja jest napisana porządnie i gdy programista rozumie ją tak samo jak autor.

I w tym kontekście również testy przychodzą z pomocą. One challengują dostarczone wymagania (albo “życzenia”) klienta. Możemy przygotowywać się do implementacji wybranej funkcji poprzez napisanie całej baterii testów definiujących pożądane zachowania. Wówczas, przed trybem “implementacji”, znajdujemy się w trybie “analizy”. Za pomocą kodu chcemy zamodelować wszystkie możliwe scenariusze. I wtedy też wykryjemy wiele braków w wymaganiach.

Ma to bardzo pozytywną konsekwencję: programista może te braki zgłosić przed napisaniem nawet linijki produkcyjnego kodu. Lepiej jest dopytać i poprosić o doprecyzowanie wymagań, niż zgadywać podczas programowania i robić błędne założenia.

Umówmy się: często decyzje czysto produktowe/funkcjonalne nie powinny być podejmowane przez programistę. Dzięki testom możemy dostarczyć klientowi (kimkolwiek by był) bardzo wczesny feedback o wybrakowanej specyfikacji.

### Testy jako komunikacja intencji

Dokumentacja i specyfikacja to nie wszystko. Testy świetnie nadają się również do mniej formalnej komunikacji między programistami!

Testy opisują działanie kodu oraz – co ważne! – intencje autora. A te intencje można zawsze zweryfikować poprzez odpalenie testów. Zatem nawet w przypadku zawiłego kodu, zrozumienie go może stać się o wiele łatwiejsze dzięki testom. One dokumentują decyzje podjęte na etapie programowania.

    Testy mogą zastąpić komentarze w kodzie

Zamiast pisać komentarz: “ten kod się wywali, jeśli podstawisz 3 zamiast 1 pod X, bo…” – piszemy testy. Jeden pod X podstawi 1 i wykona się jak trzeba. Drugi pod X podstawi 3 i będzie oczekiwał wyjątku. Ów drugi test nazywamy tak, aby nie było wątpliwości, skąd bierze się zachowanie systemu (o nazwach będziemy jeszcze mówić poniżej).

Wspominając o “komunikacji między programistami” mam na myśli również bardzo ważną komunikację ze sobą samym. “Ja – piszący kod” i “ja – czytający kod” to dwie różne osoby, mimo że w tym samym ciele. Dzieli nas czas, doświadczenie i kontekst.

Testy sprawdzają się doskonale także jako notatki na przyszłość. Nie masz w danej chwili czasu, by zaimplementować genialny pomysł? To nie problem: napisz test, by zostawić po tym pomyśle ślad w swoim kodzie… i oznacz go jako test ignorowany! Taki test nie będzie wykonywany, a dzięki odpowiedniej nazwie wskaże Twoje założenia i drogę na przyszłość.

I jeszcze jeden aspekt, możliwy do zaobserwowania szczególnie w środowisku open source (choć sprawdzi się także w “normalnej” pracy). Jeżeli używamy cudzej biblioteki i działa ona nie do końca zgodnie z naszymi oczekiwaniami, najlepszym sposobem zademonstrowania tej niezgodności będzie właśnie czerwony (nieprzechodzący) test. Jeśli jesteśmy w stanie napisać taki pokazowy test, to dyskusja wchodzi na zupełnie nowy poziom.

### Poprawa designu aplikacji

Dochodzimy do aspektu, który bardzo kocham w testach. Mogą one bowiem – jeśli odpowiednio do nich podejdziemy – znacznie poprawić design aplikacji. Nie chodzi oczywiście o jej wygląd (UI), ale o strukturę i architekturę.

Każdy testowany komponent musi udostępniać pewne API, wystawiać swoje funkcje na zewnątrz. API jest wykorzystywane zarówno przez testy, jak i przez inne komponenty składające się na całość systemu.

Stosowanie testów jednostkowych (szczególnie w kontekście TDD, ale do tego jeszcze dojdziemy) wymusza na programiście chwilę refleksji… Zamiast z pasją, zaangażowaniem, ale często (znam to z doświadczenia) bez odpowiedniego przemyślenia rzucać się w wir kodowania, twórca kodu najpierw oddaje się “krótkiej zadumie”. Ten namysł nad przyszłą implementacją – czyli nad NIEnapisanym jeszcze kodem – jest kluczowy.

Jak najlepiej to zrobić, żeby nie tylko działało, ale jeszcze było testowalne? Jak rozplanować odpowiedzialności pomiędzy klasy/komponenty? Gdzie dodać interfejs? Jakich zależności potrzebuję? Jak całość wpasuje się w już istniejący kod? I co tak naprawdę muszę napisać, żeby spełnić wymagania, nie marnując czasu na pieszczenie się z niekoniecznym kodem?

Taka chwila pozwala zrozumieć prawdziwą potrzebę programowania. Dzięki temu dobrany sposób realizacji ma szansę być tym najbardziej optymalnym i efektywnym.

    Stosowanie (dobrych) testów jednostkowych implikuje wykorzystanie zalecanych praktyk programistycznych

U mnie zainteresowanie testami jednostkowymi rozpoczęło dawno temu całkiem nowy rozdział w dev-przygodzie.

Wtedy dotarło do mnie, po kiego grzyba stosować programowanie pod interfejs/kontrakt. Wtedy też zrozumiałem, że klepanie kolejnych linii “tylko działającego” kodu ma krótkie nogi i można to zrobić lepiej. Doceniłem praktyczną wartość Inversion of Control i innych praktyk przydatnych w codziennej programistycznej pracy.

Tym samym dochodzimy do wniosku: trudny do przetestowania kod można napisać lepiej. Jasne, KAŻDY bez wyjątku kod można napisać lepiej, ale trudność testowania może być wskaźnikiem obszarów żebrzących o refactoring.

    Kod trudny do przetestowania jest kodem trudnym do utrzymania

### Narzędzie do nauki i eksploracji

Czy zdarza Ci się pisać “jednorazowe”, tymczasowe aplikacyjki konsolowe tylko w celu sprawdzenia jak działa jakiś system lub biblioteka? No to… nigdy więcej!

Takie aplikacje są potrzebne i wartościowe. A my je często po prostu kasujemy! Jak wiele doświadczenia wtedy przepada? Można zrobić to lepiej, zachowując zdobytą wiedzę w kodzie i commitując do repozytorium. Dzięki temu będziemy mogli do niej wrócić. I to nie tylko my, ale także cały zespół. I przyszłe pokolenia ;).

Takie zabawy i eksperymenty, zrealizowane oczywiście za pomocą testów, to po prostu część naszego projektu. Możemy stworzyć dedykowany folder na wybraną integrację i hulaj dusza!

W przypadku używanej biblioteki robimy coś – teoretycznie – dziwnego: dopisujemy testy do cudzego kodu! A po co? Otóż dzięki temu lepiej rozumiemy jej działanie. I sprawdzamy, czy taka biblioteka na pewno działa tak, jak nam się wydaje na podstawie dokumentacji (która – jak już wiemy z poprzednich akapitów – może być “rozjechana” z prawdziwym, żyjącym kodem).

A w bonusie otrzymujemy jeszcze jedną bardzo ważną charakterystykę, pasującą do poruszanego wcześniej tematu: regression bugs. Dzięki takim testom upewniamy się, że używany komponent działa tak samo – w kluczowych dla nas aspektach – także po aktualizacji i zmianie jego wersji! Bardzo, BARDZO przydatna świadomość i “siatka bezpieczeństwa”.

A jeśli bawimy się nie cudzą biblioteką, tylko zewnętrznym systemem, to oczywiście wychodzimy poza zakres testów jednostkowych, mocząc paluchy w testach integracyjnych. Ale to nic, bo zasada działania jest taka sama. A korzyści nawet większe, bo o ile wersję wykorzystywanej biblioteki podbijamy samodzielnie i świadomie, to działanie zewnętrznego systemu może się zmienić bez naszej wiedzy. I testy nas o tym poinformują.

## Jak pisać testy jednostkowe

Wiemy już, PO CO pisać testy. Teraz zastanówmy się, JAK to robić.

Pamiętajmy jednak, że nie ma jednej niezawodnej recepty na “dobry test”, niezależnie od kontekstu. Nie ma cudów: praktyka i identyfikowanie własnych pomyłek są najlepszym nauczycielem.

Niemniej warto znać kilka uniwersalnych rekomendacji.

Na dobry początek poślizgamy się po teorii. W każdej dziedzinie życia jesteśmy bombardowani bezsensownymi akronimami i nie inaczej jest w przypadku testów. Z tym że akurat te rekomendacje są zasadne.

Przyjrzyjmy się rekomendacjom FIRST:

### Fast

Dobry test powinien być szybki, i to w dwóch kontekstach.

Po pierwsze: szybki do uruchomienia. Jeśli uruchomienie testów będzie zajmowało dużo czasu, to po prostu nie będziemy tego robić. Ani u siebie (lokalnie), ani na build serverze.

A po drugie: powinien być szybki do przeczytania i zrozumienia. Bez tego niestety tracimy BARDZO ważny aspekt wspomniany w poprzednich akapitach, czyli dokumentację i komunikację za pomocą testów. A szkoda.

### Isolated

Test (jednostkowy) powinien być niezależny od środowiska. Od systemu plików. Bazy danych. Zewnętrznego systemu. Powinien odbywać się w ramach pojedynczego procesu, nad którym mamy całkowitą kontrolę.

O tym wspominaliśmy już wcześniej, pamiętasz?

W przeciwnym wypadku mamy do czynienia z testem integracyjnym. One też są spoko, jednak mają inne cele i kierują się innymi regułami.

### Repeatable

Różne narzędzia, runnery, a także konfiguracje systemu mogą wpływać na sposób wykonania testów. A to może z kolei spowodować, że jeśli się nie postaramy, to nie będziemy mogli na nich polegać!

Wyobraź sobie sytuację, w której wynik wykonania testów jest na Twojej maszynie inny niż na build serverze. Albo wszystko działa, chyba że odpalimy testy chwilę przed północą. Albo 29 lutego. Albo na kompletnie nowej, czystej wirtualce. Albo w losowej kolejności.

W takich wypadkach zaufanie do testów spada. A od tego momentu już jest niedaleko do tragicznego wniosku, że “to jednak nie działa, strata czasu, testy jednostkowe są fe”.

### Self-verifying

Każdy test powinien jasno określać, CO testuje: czyli zawierać tzw. asercję.

Jeśli test automatycznie nie pokazuje błędu w przypadku niespełnionych założeń, to coś jest bardzo nie tak. Niestety nieraz widziałem testy bez asercji, których jedynym zadaniem było po prostu wykonanie kodu bez jawnie zdefiniowanego założenia. Nawet jeśli celem testu jest upewnienie się, że wykonany kod nie rzuci wyjątku – też powinniśmy zrobić na to asercję!

Do takiego stanu rzeczy może doprowadzić ślepy pościg za magicznym współczynnikiem code coverage w raportach wykonania testów. Ale do niego jeszcze dojdziemy.

### Timely

Akronimy są czasami konstruowane na siłę. Pokuszę się o stwierdzenie, że autor “FIRST” koniecznie potrzebował wyrazu na “T”, żeby wszystko ładnie do siebie pasowało.

Ten ostatni czynnik można interpretować wielorako. Mi najbardziej podoba się wyjaśnienie, że testy powinny być napisane w odpowiednim momencie, czyli przed napisaniem testowanego kodu. Ten temat (nawiązanie do TDD) pojawia się już drugi raz w niniejszym tekście i już za chwilę go rozwiniemy.

## Testy to też kod!

Do poprawnej implementacji przydatnych testów konieczne jest uświadomienie sobie, że testy to kod tak samo ważny, jak kod produkcyjny! Jeśli będziemy ten aspekt programowania traktować po macoszemu, to nie możemy potem narzekać na średnie efekty pracy.

Pokusiłbym się nawet o stwierdzenie, że czasami testy są WAŻNIEJSZE niż kod produkcyjny.

Jeśli DOBRE testy piszesz na trzeźwo, to kodzik możesz pisać nawet na haju ;) (don’t try this at home).

Szczególnie na początku przygody z testami bardzo trudno jest zaakceptować taki tok myślenia. A bo to przecież jakiś osobny projekt, który sobie krąży obok “właściwego” rozwiązania i nigdy nie jest nikomu dostarczany, nie wykonuje się w środowisku produkcyjnym, nie zarabia na siebie… Błąd! Do testów powinno się podchodzić z taką samą (lub nawet większą!) dbałością jak do każdego innego elementu systemu.

Warto wiedzieć, że kodu testującego będziemy mieli więcej niż kodu testowanego… Czy cokolwiek daje nam więc prawo chuchania i dmuchania na jeden kawał rozwiązania, podczas gdy drugi kawał to paskudne spaghetti tylko dlatego, że “klient go nie uruchomi“? Ano niekoniecznie.

Trzeba zastanawiać się nad strukturą testów. Planować hierarchię klas, stosować przemyślaną architekturę, poświęcać CZAS na podnoszenie ich jakości.

W wielu projektach testy składają się w 70-80% z kodu wytworzonego metodą kopiuj/wklej. A potem jest narzekanie, że testy spowalniają pracę. No heloł, co za niespodzianka!

Jeśli kod przygotowujący środowisko do testu się powtarza, to nie kopiujmy go w dziesiątkach testów, tylko zastosujmy odpowiednie mechanizmy pozwalające na jego reużycie.

Jeśli framework wykorzystywany do testów nie zawiera pożądanej funkcjonalności, to nauczmy się go rozszerzać.

Po prostu: zadbajmy o testy. Odwdzięczą się.

Niedopuszczalna jest sytuacja, w której raz napisany test żyje sobie własnym życiem od momentu zapalenia po raz pierwszy zielonej lampki w runnerze aż “do samego końca, mojego lub jej”.

W miarę uzupełniania testów nieustannie będziemy identyfikować scenariusze powtarzalne. Trzeba wtedy postarać się przeorganizować istniejące testy w taki sposób, aby proces dodawania kolejnych mógł skorzystać z już zawartej tam wiedzy i logiki. Są do tego sprawdzone taktyki, strategie, wzorce.

Na szczególną staranność i uwagę zasługuje proces definiowania kolejnych testów. Procedura ta musi być bardzo prosta, błyskawiczna i niewymagająca wielkiego wysiłku umysłowego. Jeżeli jednak natrafimy na trudność przy tym tworzeniu nowego testu, to oznacza, że trzeba w tym miejscu się zatrzymać. Niepewność “gdzie mam ten nowy test utworzyć” pokazuje, że w naszej hierarchii musimy zdefiniować nowe miejsce na dany typ testów. Nie wsadzajmy go gdziekolwiek, bo po kilku dniach takiego postępowania: burdel murowany.

Zawahanie: “Ale jak spreparować zależności dla tego nowego testu?”, wynika z nie do końca jeszcze gotowego procesu konfiguracji konkretnego kawałka kodu. Co robimy? Bynajmniej nie piszemy masy kodu konfiguracyjnego dla tego jednego nowego scenariusza! Zamiast tego szukamy innych testów powiązanych z tą częścią programu i analizujemy, czy nowy klocek nie pasuje przypadkiem do testów już utworzonych.

I tak dalej…

A to wszystko także po to, żeby lektura testów była nieskomplikowana. Testy powinno się czytać bez ciągłego zastanawiania: “A czemu tutaj tak, a nie inaczej…?“.

## Nazewnictwo testów

## Konstruowanie testów przy użyciu AAA

### Przygotowanie - Arrange

W tej fazie tworzymy testowany obiekt, przygotowujemy dla niego środowisko i definiujemy zmienne.

Zwykle każdy test ma inny etap Arrange (bo w końcu każdy test testuje inny scenariusz).

### Wykonanie - Act

Wykonujemy testowaną logikę. Zwykle powinna to być jedna linijka. Testy zgrupowane w jednym kontenerze (klasie testującej) mają zazwyczaj identyczną fazę Act.

### Weryfikacja - Assert

Pisałem wyżej, że każdy test powinien zawierać asercję, czyli jawną weryfikację pewnego założenia.

Sprawdzamy, czy poprzednia faza wywołuje oczekiwane konsekwencje (albo nie wywołuje nieoczekiwanych).

Dobry test ma zwykle jedną (logiczną) asercję.

Na początku nauki możesz nawet pokusić się o wstawienie do pustego testu komentarzy dzielących go na te trzy fazy, by dopilnować, że na pewno nie mieszasz ich ze sobą.

# Referencje

- https://devstyle.pl/2020/06/25/mega-pigula-wiedzy-o-testach-jednostkowych/
- https://www.manning.com/books/the-art-of-unit-testing-second-edition
