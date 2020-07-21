namespace MiniORM.App
{
    using System.Linq;
    using MiniORM.App.Data.Entities;
    using Data;
    using Data.Entities;
    public class StartUp
    {
        static void Main(string[] args)
        {


            {
                static void Main(string[] args)
                {
                    var connectionString = @"Server=.;Database=MinionsDB;Integrated security = true";
                    var context = new SoftUniDbContext(connectionString);

                    context.Employees.Add(new Employee
                    {
                        FirstName = "Gosho",
                        LastName = "Inserted",
                        DepartmentId = context.Departments.First().Id,
                        IsEmployed = true,
                    });

                    var employee = context.Employees.Last();
                    employee.FirstName = "Modified";

                    context.SaveChanges();
                }
            }
        }
    }
}

