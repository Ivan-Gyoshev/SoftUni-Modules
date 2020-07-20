using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace P07._Print_All_Minion_Names
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=.;Database=MinionsDB;Integrated security = true";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            List<string> initialOrderOfMinions = new List<string>();
            List<string> arrangedOrderOfMinions = new List<string>();



            var command = new SqlCommand("SELECT Name FROM Minions", connection);

            var reader = command.ExecuteReader();

            using (reader)
            {
                if (!reader.HasRows)
                {
                    return;
                }

                while (reader.Read())
                {
                    initialOrderOfMinions.Add((string)reader["Name"]);
                }
            }


            while (initialOrderOfMinions.Count > 0)
            {

                arrangedOrderOfMinions.Add(initialOrderOfMinions[0]);
                initialOrderOfMinions.RemoveAt(0);

                if (initialOrderOfMinions.Count > 0)
                {
                    arrangedOrderOfMinions.Add(initialOrderOfMinions[initialOrderOfMinions.Count - 1]);
                    initialOrderOfMinions.RemoveAt(initialOrderOfMinions.Count - 1);
                }
            }

            foreach (var minion in arrangedOrderOfMinions)
            {
                Console.WriteLine(minion);
            }
        }
    }
}
