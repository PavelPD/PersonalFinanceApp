using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PersonalFinanceApp.Data
{
    public static class DBInitializer
    {
        private const string DataBaseFileName = "personal_finance.db";

        public static string DataBasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DataBaseFileName);

        public static SQLiteAsyncConnection GetConnection()
        {
            return new SQLiteAsyncConnection(DataBasePath);
        }

        public static async Task InitializeDatabase()
        {
            var connection = GetConnection();

            //create table Accounts
            await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS Accounts (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    balance REAL NOT NULL);
            ");

            //create table Categories
            await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS Categories (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    type TEXT NOT NULL CHECK (type IN ('income', 'expense')),
                    icon TEXT);
            ");

            //table Transactions
            await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS Transactions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    type TEXT NOT NULL CHECK (type IN ('income', 'expense')),
                    amount REAL NOT NULL,
                    account_id INTEGER NOT NULL,
                    category_id INTEGER NOT NULL,
                    comment TEXT,
                    date DATE NOT NULL,
                    FOREIGN KEY (account_id) REFERENCES Accounts(id) ON DELETE CASCADE,
                    FOREIGN KEY (category_id) REFERENCES Categories(id) ON DELETE CASCADE);
            ");

            //create table Budgets
            await connection.ExecuteAsync(@"
                    CREATE TABLE IF NOT EXISTS Budgets (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    category_id INTEGER NOT NULL,
                    amount REAL NOT NULL,
                    spent REAL NOT NULL DEFAULT 0,
                    month INTEGER NOT NULL,
                    year INTEGER NOT NULL,
                    FOREIGN KEY (category_id) REFERENCES Categories(id) ON DELETE CASCADE);
            ");
        }
    }
}
