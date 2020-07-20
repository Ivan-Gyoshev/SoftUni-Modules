using Microsoft.Data.SqlClient;
using System;

namespace P03.Minion_Names
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            int id = int.Parse(Console.ReadLine());
            var connectionString = @"Server=.;Database=MinionsDB;Integrated security = true";

            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)

                connection.Open();

            var findVillain = @"SELECT [Name] FROM Villains WHERE Id = @Id";
            SqlCommand find = new SqlCommand(findVillain, connection);
            find.Parameters.AddWithValue("Id", id);


            if (find.ExecuteScalar()?.ToString() == null)
            {
                Console.WriteLine($"No villain with ID {id} exists in the database.");
                return;
            }
            using (var reader = find.ExecuteReader())
            {
                reader.Read();
                Console.WriteLine($"Villain: {reader["Name"]}");
            }

            var findVillainMinionsString = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                         FROM MinionsVillains AS mv
                                         JOIN Minions As m ON mv.MinionId = m.Id
                                         WHERE mv.VillainId = @Id
                                         ORDER BY m.Name";

            var findMinionsInfo = new SqlCommand(findVillainMinionsString, connection);
            findMinionsInfo.Parameters.AddWithValue("Id", id);

            using (var reader = findMinionsInfo.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["Name"].ToString() == null)
                    {
                        Console.WriteLine("(no minions)");
                        continue;
                    }

                    Console.WriteLine($"{reader["RowNum"]}. {reader["Name"]} {reader["Age"]}");
                }
            }

        }
    }
}
