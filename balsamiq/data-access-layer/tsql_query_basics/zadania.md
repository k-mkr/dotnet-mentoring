1. Napisz szablon kodu, który pozwoli na obsłużenie błędów wynikających z zapytania SQL w obrębie transakcji. Dla uproszczenia możemy uznać, że chcemy wywołać zapytanie `SELECT * FROM sys.tables` w obrębie transakcji.

2. [Northwind] Przenoszenie produktów pomiędzy kategoriami.

   - Metoda powinna przyjmować ID produktu do przeniesienia oraz kategorię źródłową i docelową
   - Jeśli produkt nie istnieje w kategorii źródłowej powinniśmy rzucić wyjątek `NotFoundException`

3. [Northwind] Generowanie raportu zamówień dla określonego klienta

   - Metoda powinna przyjmować ID Klienta
   - Zbuduj strukturę raportu, która powinna zawierać:
     - Nazwę klienta
     - Szczegóły zamówień (numer zamówienia, data, produkty, ilość i cena)
   - Raport powinien być zapisany do pliku w formacie JSON

4. [Northwind] Napisz metodę, która wyciągnie wszystkie zamówienia, przy czym zmapuje je na klasę C# z polskimi nazwami:

   ```csharp
   public class Zamowienie
   {

   }
   ```
