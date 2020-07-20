using Microsoft.Data.SqlClient;
using System;

namespace P06._Remove_Villain
{
    public class StartUp
    {
        static void Main(string[] args)
        {

            var connectionString = @"Server=.;Database=MinionsDB;Integrated security = true";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            var id = int.Parse(Console.ReadLine());

            var cmd = new SqlCommand($"SELECT Name FROM Villains WHERE Id = {id}", connection);

            if (cmd.ExecuteScalar()?.ToString() == null)
            {
                Console.WriteLine("No such villain was found.");
                return;
            }

            cmd = new SqlCommand($"SELECT COUNT(*) FROM MinionsVillains WHERE VillainId = {id}", connection);
            int minionsCount = (int)cmd.ExecuteScalar();


            cmd = new SqlCommand($"DELETE FROM MinionsVillains WHERE VillainId = {id}",connection);
            

            cmd.ExecuteNonQuery();

            cmd = new SqlCommand($"SELECT Name FROM Villains WHERE Id = {id}", connection);
            var name = (string)cmd.ExecuteScalar()?.ToString();

            cmd = new SqlCommand($"DELETE FROM Villains WHERE Id = {id}",connection);
            cmd.ExecuteNonQuery();

            Console.WriteLine($"{name} was deleted.");
            Console.WriteLine($"{minionsCount} minions were released.");
        }
    }
}
