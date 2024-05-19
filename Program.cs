using System;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace _2._1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Отримання рядка підключення
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = configuration.GetConnectionString("CompanyExpensesDB");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Відображення даних з таблиці Company
                DisplayTableData(connection, "Company");

                // Додавання рядка в таблицю Company
                AddCompany(connection, 2, "Innovate Inc", "456 Innovate Street", "987-654-3210");

                // Відображення даних знову після додавання
                DisplayTableData(connection, "Company");

                // Запит з об’єднанням таблиць (JOIN)
                string joinQuery = "SELECT e.name, d.name AS department FROM Employee e JOIN Department d ON e.department_id = d.id";
                ExecuteQuery(connection, joinQuery);

                // Запит з фільтрацією
                string filterQuery = "SELECT * FROM Expense WHERE amount > 100";
                ExecuteQuery(connection, filterQuery);

                // Запит з агрегатними функціями
                string aggregateQuery = "SELECT SUM(amount) AS total_expenses FROM Expense";
                ExecuteQuery(connection, aggregateQuery);
            }
        }

        static void DisplayTableData(SqlConnection connection, string tableName)
        {
            string query = $"SELECT * FROM {tableName}";
            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            DataTable table = new DataTable();
            adapter.Fill(table);

            Console.WriteLine($"Data from {tableName}:");
            foreach (DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item} ");
                }
                Console.WriteLine();
            }
        }

        static void AddCompany(SqlConnection connection, int id, string name, string address, string phone)
        {
            string query = "INSERT INTO Company (id, name, address, phone) VALUES (@id, @name, @address, @phone)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@address", address);
                command.Parameters.AddWithValue("@phone", phone);
                command.ExecuteNonQuery();
            }
        }

        static void ExecuteQuery(SqlConnection connection, string query)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader[i]} ");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}
