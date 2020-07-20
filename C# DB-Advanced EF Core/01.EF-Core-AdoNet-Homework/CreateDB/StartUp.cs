using Microsoft.Data.SqlClient;
using System;

namespace AdoNetHomework
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=.;Database=MinionsDB;Integrated security = true";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            var createCommand = "CREATE DATABASE MinionsDB";

            using SqlCommand command = new SqlCommand(createCommand,connection);

            command.ExecuteNonQuery();
        }
    }
}
