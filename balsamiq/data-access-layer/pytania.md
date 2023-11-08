# Pytania Sprawdzające

**1. Co to jest SqlConnection i jak jest używane w programowaniu z użyciem języka C# i technologii .NET?**

`SqlConnection` jest klasą w bibliotece ADO.NET, która reprezentuje połączenie z bazą danych SQL Server. Służy do nawiązywania i zarządzania połączeniami z bazą danych w programowaniu z użyciem języka C# i technologii .NET.

**2. Jakie są etapy otwarcia i zamknięcia połączenia SqlConnection?**

Etapy otwarcia i zamknięcia połączenia SqlConnection to:

- Otwarcie połączenia: Aby otworzyć połączenie, należy utworzyć nową instancję SqlConnection i użyć metody Open().
- Zamknięcie połączenia: Aby zamknąć połączenie, należy użyć metody Close() lub Dispose() na obiekcie SqlConnection. Zamknięcie połączenia jest ważne, aby zwolnić zasoby i zakończyć połączenie z bazą danych.

**3. Jakie są korzyści z używania parametrów SqlParameter zamiast konkatenacji ciągów w zapytaniach SQL?**

Używanie parametrów SqlParameter zamiast konkatenacji ciągów w zapytaniach SQL ma kilka korzyści:

- Zabezpieczenie przed atakami typu SQL Injection.
- Poprawa wydajności, ponieważ baza danych może skompilować zapytanie tylko raz i używać go wielokrotnie z różnymi parametrami.
- Obsługa prawidłowych typów danych, bez konieczności konwersji i formatowania wartości ręcznie.

**4. Jakie są najważniejsze właściwości i metody klasy SqlCommand?**

Właściwości klasy SqlCommand:

- CommandText: Określa polecenie SQL, które ma zostać wykonane.
- CommandType: Określa typ polecenia, na przykład tekstowy (CommandType.Text) lub procedura składowana (CommandType.StoredProcedure).
- Connection: Określa połączenie SqlConnection, z którym jest powiązany obiekt SqlCommand.
- Parameters: Przechowuje kolekcję parametrów SqlParameter dla polecenia SQL.

Metody klasy SqlCommand obejmują ExecuteNonQuery(), ExecuteScalar(), ExecuteReader() i inne, które służą do wykonania różnych rodzajów zapytań i operacji na bazie danych.

**5. Jakie znasz metody klasy SqlCommand i kiedy każda z nich powinna być używana?**

- ExecuteNonQuery():

  Ta metoda jest używana, gdy chcemy wykonać polecenie SQL, które nie zwraca wyników, takie jak INSERT, UPDATE, DELETE itp.
  Metoda zwraca liczbę wierszy dotkniętych przez wykonane polecenie.

  ```csharp
  SqlCommand command = new SqlCommand("UPDATE Customers SET City='Warsaw' WHERE Country='Poland'", connection);
  int rowsAffected = command.ExecuteNonQuery();
  ```

- ExecuteScalar():

  Ta metoda jest używana, gdy oczekujemy pojedynczej wartości wynikowej, np. po wykonaniu funkcji agregującej w zapytaniu SQL.
  Metoda zwraca pojedynczą wartość z pierwszej kolumny pierwszego wiersza wynikowego zbioru danych.

  ```csharp
  SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Customers", connection);
  int count = (int)command.ExecuteScalar();
  ```

- ExecuteReader():

  Ta metoda jest używana, gdy chcemy wykonać zapytanie, które zwraca zbiór wyników w postaci obiektu SqlDataReader.
  Metoda zwraca obiekt SqlDataReader, który można używać do iteracji przez wyniki zapytania.

  ```csharp
  SqlCommand command = new SqlCommand("SELECT * FROM Customers", connection);
  SqlDataReader reader = command.ExecuteReader();
  while (reader.Read())
  {
      // Obsługa wyników
  }
  reader.Close();
  ```

- ExecuteXmlReader():

  Ta metoda jest używana, gdy chcemy wykonać zapytanie, które zwraca dane w formacie XML.
  Metoda zwraca obiekt XmlReader, który można używać do odczytu danych w formacie XML.

  ```csharp
  SqlCommand command = new SqlCommand("SELECT * FROM Customers FOR XML AUTO", connection);
  XmlReader xmlReader = command.ExecuteXmlReader();
  while (xmlReader.Read())
  {
      // Obsługa danych XML
  }
  xmlReader.Close();
  ```

**6. W jaki sposób korzystać z transakcji SQL za pomocą klasy SqlTransaction w C#?**

Do obsługi transakcji SQL w C# można użyć klasy `SqlTransaction`. Najpierw należy utworzyć obiekt `SqlTransaction` na podstawie obiektu `SqlConnection`. Następnie można rozpocząć transakcję, wykonywać zapytania SQL, a na koniec zatwierdzić (`Commit`) lub wycofać (`Rollback`) transakcję w zależności od rezultatu operacji.

**7. Jak obsłużyć błędy związane z połączeniem, zapytaniem SQL lub transakcją przy użyciu try-catch-finally bloków?**

Aby obsłużyć błędy związane z połączeniem, zapytaniem SQL lub transakcją, można użyć bloków try-catch-finally. W bloku try, umieścić kod, który może generować wyjątki. W bloku catch, obsłużyć konkretne wyjątki i podjąć odpowiednie działania, takie jak wyświetlanie komunikatu o błędzie lub cofnięcie transakcji. Blok finally wykona się zawsze

**8. Jakie są dobre praktyki, których należy przestrzegać pracując z transakcjami?**

**9. Jakie są główne cechy biblioteki Dapper?**

- Szybkość i wydajność: Dapper jest znacznie szybszy od tradycyjnych metod ORM, ponieważ minimalizuje narzut mapowania obiektów.
- Prostota użycia: Dapper oferuje proste API, które jest łatwe do zrozumienia i używania.
- Surowe zapytania SQL: Dapper pozwala na pisanie surowych zapytań SQL, co daje większą kontrolę nad operacjami na bazie danych.
- Automatyczne mapowanie wyników: Dapper automatycznie mapuje wyniki zapytań SQL na obiekty C#, eliminując konieczność ręcznego mapowania.

**10. W jaki sposób Dapper ułatwia mapowanie wyników zapytań SQL na obiekty C#?**

Dapper ułatwia mapowanie wyników zapytań SQL na obiekty C# poprzez automatyczne dopasowanie nazw kolumn wynikowych do właściwości obiektów. Można również używać adnotacji lub konfiguracji ręcznej, aby precyzyjnie określić mapowanie.

**11. Jakie są podstawowe kroki do wykonywania zapytań SQL przy użyciu Dapper?**

- Utworzenie obiektu połączenia SqlConnection.
- Napisanie zapytania SQL z odpowiednimi parametrami.
- Wywołanie metody `Query`, `Execute` lub `ExecuteScalar`, przekazując zapytanie SQL oraz opcjonalne parametry.
- Przechwycenie wyników zapytania i przetworzenie ich zgodnie z potrzebami.

**12. Co to jest Dapper.Contrib i jakie funkcjonalności oferuje w stosunku do podstawowej wersji Dapper?**

Dapper.Contrib to rozszerzenie biblioteki Dapper, które dodaje funkcjonalności ORM do Dapper. Oferuje ono m.in. automatyczne mapowanie obiektów na tabele w bazie danych, wsparcie dla operacji CRUD oraz obsługę relacji między tabelami.

**13. Jakie są zalety używania Dapper.Contrib w porównaniu z ręcznym tworzeniem zapytań SQL w Dapper?**

-Automatyczne mapowanie obiektów na tabele i kolumny w bazie danych.
-Wygodne operacje CRUD (tworzenie, odczyt, aktualizacja, usuwanie) na tabelach.
-Obsługa relacji między tabelami za pomocą adnotacji i konwencji.

**14. W jaki sposób Dapper i Dapper.Contrib obsługują relacje między tabelami w bazie danych?**

Dapper i Dapper.Contrib obsługują relacje między tabelami w bazie danych poprzez adnotacje, takie jak [Key], [ExplicitKey], [Computed] i [Write]. Te adnotacje pozwalają określić klucze główne, klucze obce i inne zależności między tabelami.

**15. Jak Dapper obsługuje transakcje i jak można wykonywać operacje z użyciem transakcji przy użyciu Dapper?**

Dapper obsługuje transakcje poprzez użycie obiektu SqlTransaction. Można rozpocząć transakcję, wykonać operacje na bazie danych, a następnie zatwierdzić (Commit) lub wycofać (Rollback) transakcję w zależności od wyniku operacji.

**16. Jak można dostosować mapowanie obiektów i tabel w Dapper i Dapper.Contrib?**

Mapowanie obiektów i tabel w Dapper i Dapper.Contrib można dostosować za pomocą adnotacji, konwencji, mapowań ręcznych i rozszerzeń. Można określić nazwy tabel, kolumn, kluczy głównych i innych szczegółów mapowania, aby dostosować sposób, w jaki Dapper mapuje obiekty na bazę danych.

**17. Jak używać Dapper.Contrib do wykonywania operacji CRUD (tworzenie, odczyt, aktualizacja, usuwanie) na tabelach w bazie danych?**

Dapper.Contrib umożliwia wykonywanie operacji CRUD na tabelach w bazie danych za pomocą metod takich jak `Insert`, `Update`, `Delete`, `Get` i `GetAll`. Te metody przyjmują obiekty jako argumenty i automatycznie mapują je na odpowiednie tabele i kolumny.
