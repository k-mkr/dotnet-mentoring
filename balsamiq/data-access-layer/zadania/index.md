# Northwind

1. Wyświetl wszystkich klientów pochodzących z Niemiec.

```sql
SELECT * FROM Customers WHERE Country = 'Germany';
```

2. Wyświetl imię i nazwisko wszystkich pracowników wraz z datą ich zatrudnienia oraz ilością przepracowanych lat.

```sql
SELECT
 FirstName,
 LastName,
 HireDate,
 DATEDIFF(YEAR, HireDate, CURRENT_TIMESTAMP) AS AgeSinceHired
FROM Employees;
```

3. Wyświetl produkty, których cena jednostkowa przekracza 50$.

```sql
SELECT * FROM Products WHERE UnitPrice > 50;
```

4. Znajdź całkowitą liczbę zamówień dla każdego klienta.

```sql
SELECT
  Customers.CustomerID,
  Customers.CompanyName,
  COUNT(Orders.OrderID) AS TotalOrders
FROM
  Customers
  LEFT JOIN Orders ON Customers.CustomerID = Orders.CustomerID
GROUP BY
  Customers.CustomerID,
  Customers.CompanyName;
```

5. Wyświetl wszystkie produkty, wraz z ich kategorią oraz nazwą dostawcy

```sql
SELECT
  Products.ProductName,
  Categories.CategoryName,
  Suppliers.CompanyName
FROM
  Products
  JOIN Categories ON Products.CategoryID = Categories.CategoryID
  JOIN Suppliers ON Products.SupplierID = Suppliers.SupplierID;
```

6. Oblicz średnią wartość zamówień poszczególnych klientów. Uwzględnij przy tym naliczoną zniżkę

```sql
SELECT
  Customers.CustomerID,
  Customers.CompanyName,
  AVG(
    OrderDetails.Quantity * OrderDetails.UnitPrice * (1 - OrderDetails.Discount)
  ) AS AverageQuantity
FROM
  Customers
  JOIN Orders ON Customers.CustomerID = Orders.CustomerID
  JOIN OrderDetails ON Orders.OrderID = OrderDetails.OrderID
GROUP BY
  Customers.CustomerID,
  Customers.CompanyName;
```

7. Znajdź 5 klientów, których wartość wszystkich zamówień jest najwyższa.

```sql
SELECT
  TOP 5 Customers.CustomerID,
  Customers.CompanyName,
  SUM(
    OrderDetails.Quantity * OrderDetails.UnitPrice
  ) AS TotalAmount
FROM
  Customers
  JOIN Orders ON Customers.CustomerID = Orders.CustomerID
  JOIN OrderDetails ON Orders.OrderID = OrderDetails.OrderID
GROUP BY
  Customers.CustomerID,
  Customers.CompanyName
ORDER BY
  TotalAmount DESC
```

8. Wyświetl wszystkich unikalnych klientów pochodzących z USA, którzy złożyli zamówienie w roku 1997

```sql
SELECT
  DISTINCT Customers.CustomerID,
  Customers.CompanyName
FROM
  Customers
  JOIN Orders ON Customers.CustomerID = Orders.CustomerID
WHERE
  Customers.Country = 'USA'
  AND YEAR(Orders.OrderDate) = 1997
```

9. Znajdź wszystkich pracowników, którzy mają w nazwie stanowiska "Sales" i raportują do "Sales Manager"

```sql
SELECT
  *
FROM
  Employees
WHERE
  Title LIKE '%Sales%'
  AND ReportsTo IN (
    SELECT
      EmployeeID
    FROM
      Employees
    WHERE
      Title = 'Sales Manager'
  )
```

10. Znajdź wszystkie produkty, które zostały zamówione przez klientów z Francji.

```sql
SELECT
  DISTINCT Products.ProductName
FROM
  Products
  JOIN OrderDetails ON Products.ProductID = OrderDetails.ProductID
  JOIN Orders ON OrderDetails.OrderID = Orders.OrderID
  JOIN Customers ON Orders.CustomerID = Customers.CustomerID
WHERE
  Customers.Country = 'France';
```

11. Wyświetl wszystkich klientów, którzy nie złożyli ani jednego zamówienia.

```sql
SELECT
  Customers.CompanyName
FROM
  Customers
  LEFT JOIN Orders ON Customers.CustomerID = Orders.CustomerID
WHERE
  Orders.OrderID IS NULL;
```

12. Wyświetl wszystkich pracowników, którzy wykonali zamówienia we wszystkich państwach.

```sql
SELECT
  Employees.FirstName,
  Employees.LastName
FROM
  Employees
  JOIN Orders ON Employees.EmployeeID = Orders.EmployeeID
GROUP BY
  Employees.FirstName,
  Employees.LastName
HAVING
  COUNT(DISTINCT Orders.ShipCountry) = (
    SELECT
      COUNT(DISTINCT ShipCountry)
    FROM
      Orders
  );
```

13. Wyświetl produkty, które zostały zamówione przez klientów z przynajmniej piętnastu różnych krajów.

```sql
SELECT
  DISTINCT Products.ProductID
FROM
  Products
  JOIN OrderDetails ON Products.ProductID = OrderDetails.ProductID
  JOIN Orders ON OrderDetails.OrderID = Orders.OrderID
  JOIN Customers ON Orders.CustomerID = Customers.CustomerID
GROUP BY
  Products.ProductID
HAVING
  COUNT(DISTINCT Customers.Country) >= 15;
```

14. Wyświetl pierwsze trzy najdroższe produkty w każdej kategorii.

```sql
SELECT
  Products.ProductName,
  Categories.CategoryName,
  Products.UnitPrice
FROM
  Products
  JOIN Categories ON Products.CategoryID = Categories.CategoryID
WHERE
  (
    SELECT
      COUNT(*)
    FROM
      Products p
    WHERE
      p.CategoryID = Products.CategoryID
      AND p.UnitPrice > Products.UnitPrice
  ) < 3
ORDER BY
  Categories.CategoryName,
  Products.UnitPrice DESC;
```

15. Wyświetl pierwszych trzech klientów, których wartość zamówień jest największa w każdej z kategorii.

```sql
SELECT
  CategoryPurchases.CategoryName,
  CategoryPurchases.CustomerID,
  Customers.CompanyName,
  CategoryPurchases.TotalPurchases
FROM
  (
    SELECT
      Categories.CategoryName,
      Orders.CustomerID,
      SUM(
        OrderDetails.Quantity * OrderDetails.UnitPrice
      ) AS TotalPurchases,
      ROW_NUMBER() OVER (
        PARTITION BY Categories.CategoryID
        ORDER BY
          SUM(
            OrderDetails.Quantity * OrderDetails.UnitPrice
          ) DESC
      ) AS Rank
    FROM
      OrderDetails
      JOIN Products ON OrderDetails.ProductID = Products.ProductID
      JOIN Categories ON Products.CategoryID = Categories.CategoryID
      JOIN Orders ON OrderDetails.OrderID = Orders.OrderID
    GROUP BY
      Categories.CategoryID,
      Categories.CategoryName,
      Orders.CustomerID
  ) AS CategoryPurchases
  JOIN Customers ON CategoryPurchases.CustomerID = Customers.CustomerID
WHERE
  CategoryPurchases.Rank <= 3;
```

16. Implement an inventory management system to track product quantities and update stock levels when new orders are placed.
