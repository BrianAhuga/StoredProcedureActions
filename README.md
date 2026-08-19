# StoredProcedureActions

A practical **ASP.NET Core application demonstrating how to work with SQL Server stored procedures from a .NET application**.

The project uses the **Northwind (`NORTHWND`) database** as its testing database and demonstrates how stored procedures can be used to retrieve customers, orders, order details, top-selling products, and create new customers.

The repository includes both the **Northwind database backup** and a **sample SQL script for creating the stored procedures** required by the application.

## Overview

**StoredProcedureActions** is a backend-focused project created to demonstrate integration between an ASP.NET Core application and SQL Server stored procedures.

Instead of performing all database operations through Entity Framework Core or inline SQL queries, the application communicates with predefined stored procedures in SQL Server.

The project demonstrates both **read and write operations**, including querying related data, calculating totals, filtering results, and inserting new records using transactions.

## Features

* Execute SQL Server stored procedures from ASP.NET Core
* Retrieve customers by country
* Retrieve orders for a specific customer
* Calculate order totals
* Retrieve detailed order information
* Calculate individual order line totals
* Retrieve top-selling products
* Filter product sales by date range
* Create new customers
* Duplicate customer validation
* SQL Server transactions
* SQL Server error handling
* Parameterized stored procedure execution
* Northwind database integration
* REST API integration

## Technology Stack

| Technology               | Purpose                      |
| ------------------------ | ---------------------------- |
| **C#**                   | Primary programming language |
| **ASP.NET Core**         | Backend framework            |
| **SQL Server**           | Database                     |
| **Stored Procedures**    | Database operations          |
| **ADO.NET / SQL Client** | Database communication       |
| **Northwind**            | Testing database             |
| **REST API**             | Application interface        |

## Project Architecture

The application follows a straightforward backend architecture where requests are processed by the ASP.NET Core application and database operations are delegated to SQL Server stored procedures.

```text
Client
   │
   ▼
ASP.NET Core Application
   │
   ▼
Data Access / Service Layer
   │
   ▼
SQL Server Stored Procedures
   │
   ▼
NORTHWND Database
```

This approach demonstrates how a .NET application can work with an existing database where business and query logic is implemented through stored procedures.

## Database

The repository contains the **Northwind database** in:

```text
northwind.zip
```

The database is restored as:

```text
NORTHWND
```

The project uses several of the standard Northwind tables, including:

* `Customers`
* `Orders`
* `Order Details`
* `Products`

### Restoring the Database

1. Download or clone the repository.
2. Locate `northwind.zip`.
3. Extract the archive.
4. Restore the included database backup using **SQL Server Management Studio**.
5. Confirm that the database is available as:

```text
NORTHWND
```

Once restored, the database can be used to test the stored procedures and the ASP.NET Core application.

## Sample Stored Procedures

The repository includes a sample SQL script containing the stored procedures required for the project.

The script should be executed against the `NORTHWND` database.

Open SQL Server Management Studio and select the database:

```sql
USE [NORTHWND];
GO
```

Then execute the provided stored-procedure script.

The script creates five stored procedures.

---

## 1. Get Customers by Country

```text
dbo.usp_GetCustomersByCountry
```

Retrieves customers based on a specified country.

### Parameter

```text
@Country NVARCHAR(50)
```

### Example

```sql
EXEC dbo.usp_GetCustomersByCountry
    @Country = 'UK';
```

The procedure returns:

* Customer ID
* Company name
* Contact name
* Contact title
* Address
* City
* Region
* Postal code
* Country
* Phone
* Fax

Results are ordered alphabetically by company name.

---

## 2. Get Orders by Customer

```text
dbo.usp_GetOrdersByCustomer
```

Retrieves orders belonging to a specific customer and calculates the total value of each order.

### Parameter

```text
@CustomerID NCHAR(5)
```

### Example

```sql
EXEC dbo.usp_GetOrdersByCustomer
    @CustomerID = 'ALFKI';
```

The procedure returns:

* Order ID
* Order date
* Required date
* Shipped date
* Shipping country
* Freight
* Order total

The order total is calculated using:

```text
Unit Price × Quantity × (1 - Discount)
```

---

## 3. Get Order Details

```text
dbo.usp_GetOrderDetails
```

Retrieves individual product lines for a specific order.

### Parameter

```text
@OrderID INT
```

### Example

```sql
EXEC dbo.usp_GetOrderDetails
    @OrderID = 10248;
```

The procedure joins the `Order Details` and `Products` tables and returns:

* Order ID
* Product ID
* Product name
* Unit price
* Quantity
* Discount
* Line total

The line total is calculated using:

```text
Unit Price × Quantity × (1 - Discount)
```

---

## 4. Get Top-Selling Products

```text
dbo.usp_GetTopSellingProducts
```

Retrieves the best-selling products based on total quantity sold.

### Parameters

```text
@TopN INT = 10
@StartDate DATE = NULL
@EndDate DATE = NULL
```

Both dates are optional.

### Example

Get the top 10 products:

```sql
EXEC dbo.usp_GetTopSellingProducts
    @TopN = 10;
```

Get the top 5 products within a specific period:

```sql
EXEC dbo.usp_GetTopSellingProducts
    @TopN = 5,
    @StartDate = '1996-01-01',
    @EndDate = '1997-12-31';
```

The procedure returns:

* Product ID
* Product name
* Total quantity sold
* Total revenue

The end date is treated as **inclusive**.

---

## 5. Create a New Customer

```text
dbo.usp_CreateCustomer
```

Creates a new customer in the `Customers` table.

Before inserting the record, the procedure checks whether the supplied `CustomerID` already exists.

### Example

```sql
EXEC dbo.usp_CreateCustomer
    @CustomerID = 'TEST1',
    @CompanyName = 'Test Company',
    @ContactName = 'John Doe',
    @Country = 'Kenya';
```

The procedure demonstrates:

* Duplicate record checking
* Parameterized insertion
* Database transactions
* Transaction rollback
* Error handling
* Returning the newly created record

> **Warning:** This procedure modifies the database. Use a development or test copy of the Northwind database when experimenting with it.

## Error Handling

The stored procedures use SQL Server's `TRY...CATCH` pattern:

```sql
BEGIN TRY
    -- Database operation
END TRY
BEGIN CATCH
    THROW;
END CATCH
```

This allows database errors to be propagated back to the application.

The customer creation procedure additionally uses transactions:

```sql
BEGIN TRANSACTION;

-- Insert customer

COMMIT TRANSACTION;
```

If an error occurs, the transaction is rolled back:

```sql
IF XACT_STATE() <> 0
    ROLLBACK TRANSACTION;

THROW;
```

This demonstrates how transactional database operations can be handled safely.

## Getting Started

### Prerequisites

Make sure you have the following installed:

* [.NET SDK](https://dotnet.microsoft.com/download)
* Microsoft SQL Server
* SQL Server Management Studio
* Visual Studio or Visual Studio Code
* Git

### 1. Clone the Repository

```bash
git clone https://github.com/BrianAhuga/StoredProcedureActions.git
```

Navigate into the project:

```bash
cd StoredProcedureActions
```

### 2. Restore the Northwind Database

Locate:

```text
northwind.zip
```

Extract the database backup and restore it using SQL Server Management Studio.

Confirm that the database is named:

```text
NORTHWND
```

### 3. Create the Stored Procedures

Open the sample SQL script included in the repository.

Connect to SQL Server and execute the script against:

```sql
USE [NORTHWND];
GO
```

This will create:

```text
usp_GetCustomersByCountry
usp_GetOrdersByCustomer
usp_GetOrderDetails
usp_GetTopSellingProducts
usp_CreateCustomer
```

### 4. Configure the Connection String

Update the application's `appsettings.json` with your SQL Server connection string.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=NORTHWND;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Replace `YOUR_SERVER` with your SQL Server instance.

For SQL Server authentication:

```text
Server=YOUR_SERVER;
Database=NORTHWND;
User Id=YOUR_USERNAME;
Password=YOUR_PASSWORD;
TrustServerCertificate=True;
```

> **Security:** Do not commit production database credentials to GitHub. Use User Secrets, environment variables, or another secure secrets-management solution when working with sensitive credentials.

### 5. Restore .NET Dependencies

```bash
dotnet restore
```

### 6. Build the Application

```bash
dotnet build
```

### 7. Run the Application

```bash
dotnet run
```

The application will start using the configured ASP.NET Core development environment.

## Example Stored Procedure Workflow

A typical request follows this flow:

```text
Client Request
      │
      ▼
ASP.NET Core Endpoint
      │
      ▼
Application Code
      │
      │ Parameters
      ▼
SQL Server Stored Procedure
      │
      ▼
NORTHWND Database
      │
      ▼
Query Result
      │
      ▼
ASP.NET Core Response
      │
      ▼
Client
```

For example:

```text
GET Customers by Country
        │
        ▼
usp_GetCustomersByCountry
        │
        ▼
Customers Table
        │
        ▼
Customer Results
```

## Learning Objectives

This project provides practical experience with:

* C#
* ASP.NET Core
* REST API development
* SQL Server
* Stored procedures
* ADO.NET
* Parameterized database operations
* SQL joins
* Aggregate functions
* Date filtering
* Transactions
* SQL error handling
* Database-driven application development
* Mapping database results to application models

## Why Stored Procedures?

Stored procedures are still widely encountered in enterprise and database-heavy applications.

They can be useful for:

* Centralizing database logic
* Reusing complex queries
* Encapsulating database operations
* Managing transactional operations
* Working with existing or legacy databases
* Controlling database access
* Optimizing frequently executed database operations

This project provides practical experience working with a **database-first approach**, which is particularly useful when integrating with existing enterprise systems.

## Security Considerations

When implementing stored-procedure-based applications in production:

* Always use parameterized inputs.
* Never concatenate user input into SQL statements.
* Protect database credentials.
* Use least-privilege database accounts.
* Validate incoming parameters.
* Use transactions where atomic operations are required.
* Avoid exposing raw database errors to users.
* Keep sensitive configuration outside source control.
* Use HTTPS for application communication.

## Future Improvements

Potential improvements include:

* Swagger/OpenAPI documentation
* Complete CRUD stored procedures
* Async database operations
* Repository and service abstractions
* Unit testing
* Integration testing
* Structured application logging
* Global exception handling
* Authentication and authorization
* Pagination
* Advanced reporting endpoints
* Docker-based SQL Server development environment
* Automated database initialization
* CI/CD integration

## Author

**Brian Ahuga**

Software Engineer specializing in scalable software systems, backend services, enterprise applications, and database-driven solutions.

GitHub: [BrianAhuga](https://github.com/BrianAhuga)

## License

This project is intended for learning, experimentation, and portfolio demonstration.
