using Microsoft.Data.SqlClient;
using System;

namespace P02.Villain_Names
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=.;Database=MinionsDB;Integrated security = true";

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            var commandString = @"SELECT v.[Name], COUNT(MinionId) AS MinionsCount FROM Villains AS v
                                 JOIN MinionsVillains AS mv
                                 ON mv.VillainId = v.Id
                                 GROUP BY v.Name
                                 HAVING COUNT(MinionId) > 3
                                 ORDER BY MinionsCount DESC";

            using SqlCommand command = new SqlCommand(commandString, connection);

            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var name = reader["Name"];
                var count = reader["MinionsCount"];

                Console.WriteLine(name + " - " + count);
            }

        }
    }
}
