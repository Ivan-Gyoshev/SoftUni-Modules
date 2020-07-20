using Microsoft.Data.SqlClient;
using System;

namespace P04.Add_Minion
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=.;Database=MinionsDB;Integrated security = true";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            var minionInfo = Console.ReadLine().Split();
            var minionName = minionInfo[1];
            var minionAge = int.Parse(minionInfo[2]);
            var minionTown = minionInfo[3];

            var villainInput = Console.ReadLine().Split();
            var villainName = villainInput[1];

            var command = new SqlCommand($"SELECT COUNT(*) FROM Towns WHERE Name = '{minionTown}'", connection);

            if ((int)command.ExecuteScalar() == 0)
            {
                command = new SqlCommand($"INSERT INTO Towns(Name) VALUES ('{minionTown}')", connection);
                command.ExecuteNonQuery();
                Console.WriteLine($"Town {minionTown} was added to the database.");
            }

            command = new SqlCommand($"SELECT COUNT(*) FROM Villains WHERE Name = '{villainName}'", connection);

            if ((int)command.ExecuteScalar() == 0)
            {
                command = new SqlCommand($"INSERT INTO Villains (Name, EvilnessFactorId) VALUES ('{villainName}', 4)", connection);
                command.ExecuteNonQuery();
                Console.WriteLine($"Villain {villainName} was added to the database.");
            }

            command = new SqlCommand($"SELECT Id FROM Towns WHERE Name = '{minionTown}'", connection);
            int townId = (int)command.ExecuteScalar();

            command = new SqlCommand($"INSERT INTO Minions(Name, Age, TownId) VALUES ('{minionName}', {minionAge}, {townId})", connection);
            command.ExecuteNonQuery();

            int villainId = (int)new SqlCommand($"SELECT Id FROM Villains WHERE Name = '{villainName}'", connection).ExecuteScalar();
            int minionId = (int)new SqlCommand($"SELECT Id FROM Minions WHERE Name = '{minionName}'", connection).ExecuteScalar();

            command = new SqlCommand($"INSERT INTO MinionsVillains VALUES ({minionId}, {villainId})", connection);
            command.ExecuteNonQuery();
            Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
        }
    }
}
