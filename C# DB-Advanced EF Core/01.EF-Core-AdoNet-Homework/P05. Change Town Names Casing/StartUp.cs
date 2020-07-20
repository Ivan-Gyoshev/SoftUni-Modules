using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace P05._Change_Town_Names_Casing
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=.;Database=MinionsDB;Integrated security = true";

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            var country = Console.ReadLine();

            var countryCommand = @" SELECT t.Name 
                                 FROM Towns as t
                                 JOIN Countries AS c ON c.Id = t.CountryCode
                                 WHERE c.Name = @countryName";

            var check = new SqlCommand(countryCommand, connection);
            check.Parameters.AddWithValue("countryName", country);

            if (check.ExecuteScalar()?.ToString() == null)
            {
                Console.WriteLine("No town names were affected.");
                return;
            }

         
            var count = 0;
            var list = new List<string>();
            var reader = check.ExecuteReader();
            while (reader.Read())
            {
                var townNameBefore = reader["Name"].ToString();
                var townNameAfter = reader["Name"].ToString().ToUpper().ToString();

                if (townNameAfter == townNameBefore)
                {
                    continue;
                }

                new SqlCommand($"UPDATE Towns SET Name = '{reader["Name"].ToString().ToUpper()}", connection);
                list.Add(townNameAfter);
                count++;

            }

            if (count == 0)
            {
                Console.WriteLine("No town names were affected.");
            }
            else
            {
                Console.WriteLine($"{count} town names were affected.");
                Console.WriteLine($"[{string.Join(", ", list)}]");
            }
        }
    }
}
