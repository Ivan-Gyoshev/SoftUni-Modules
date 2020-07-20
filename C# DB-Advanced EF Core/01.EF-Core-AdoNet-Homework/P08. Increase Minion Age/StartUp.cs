using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace P08._Increase_Minion_Age
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=.;Database=MinionsDB;Integrated security = true";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            var ids = Console.ReadLine().Split(" ");

            for (int i = 0; i < ids.Length; i++)
            {
                var command = new SqlCommand($" UPDATE Minions " +
                                            $"SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1 " +
                                            $"WHERE Id = {int.Parse(ids[i])}", connection).ExecuteNonQuery();


            }

            var cmd = new SqlCommand($"SELECT Name, Age FROM Minions", connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Name"]} {reader["Age"]}");
            }
        }
    }
}
