# Personal Finance Manager

**Version: 1.0.0**

Personal Finance Manager is a simple web application for managing personal savings and income.

I originally developed this project for my own personal use to manage my income, savings, bank accounts, transactions, and financial records. I decided to make it public so that other developers or users might find it useful and use it as a starting point for their own projects.

## Features

* Personal income management
* Savings management
* Bank account management
* Deposit and withdrawal records
* Bank account transaction history
* Personal notes
* Basic application settings
* User registration and authentication
* Cookie-based authentication

## Technologies

* **ASP.NET Core / .NET 8**
* **Entity Framework Core**
* **SQLite**
* **SQL Server**
* **jQuery**
* **Bootstrap**

## Database Structure

The application currently uses two database contexts for two different purposes.

### SQLite Database

The main personal finance data is stored in a local SQLite database.

The database file is included in the project under:

```text
db/hesabdarDB.db
```

The `ApplicationDbContext` manages the main application data:

```text
ApplicationDbContext
├── tbl_bank_account
├── tbl_deposit_or_withdraw
├── tbl_note
├── tbl_option
└── tbl_history_bank_account
```

This database contains the user's personal financial information.

### SQL Server Database

SQL Server is currently used for user registration and authentication.

The `SqlServerDbContext` manages:

```text
SqlServerDbContext
└── tbl_register
```

The SQL Server database is intended to act as the central database for user registration, while the user's personal financial information is stored locally in SQLite.

## Database Setup

The current project expects the registration table to be available in SQL Server.

If you want to run the project without a separate SQL Server database, you can move the `tbl_register` table into the SQLite database and modify the application so that both contexts use a single `ApplicationDbContext`.

In that case, the database structure could be:

```text
SQLite
└── hesabdarDB.db
    ├── tbl_register
    ├── tbl_bank_account
    ├── tbl_deposit_or_withdraw
    ├── tbl_note
    ├── tbl_option
    └── tbl_history_bank_account
```

You would then remove the separate `SqlServerDbContext` and use a single `DbContext` for all tables.

## Getting Started

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Make sure .NET 8 SDK is installed.
4. The SQLite database is available in:

```text
db/hesabdarDB.db
```

5. Configure the SQL Server connection string if you want to keep user registration in SQL Server.
6. Build and run the application.

## Notes

This project was originally developed for personal use, so some parts of the code and database structure may reflect the requirements of that original use case.

The project is published primarily as a practical example and starting point for developers who are interested in ASP.NET Core, Entity Framework Core, SQLite, and personal finance applications.

## License

© Mojtaba Golnouri  
GitHub: https://github.com/golnouri

