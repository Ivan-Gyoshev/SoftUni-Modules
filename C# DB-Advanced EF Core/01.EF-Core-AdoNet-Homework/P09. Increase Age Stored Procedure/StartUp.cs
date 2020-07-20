using Microsoft.Data.SqlClient;
using System;

namespace P09._Increase_Age_Stored_Procedure
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=.;Database=MinionsDB;Integrated security = true";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            int id = int.Parse(Console.ReadLine());


            var command = new SqlCommand($"CREATE PROC usp_GetOlder {id} INT", connection);
            command.ExecuteNonQuery();

            command = new SqlCommand($"SELECT * " +
                                     $"FROM Minions " +
                                     $"WHERE Id = {id}", connection);

            using var reader = command.ExecuteReader();

            reader.Read();
            Console.WriteLine($"{(string)reader["Name"]} - {(int)reader["Age"]} years old");

        }
    }
}
