using System;
using System.Text;
using System.Linq;
using SoftUni.Data;
using Microsoft.EntityFrameworkCore;
using SoftUni.Models;
using System.Runtime.Loader;
using System.Collections.Generic;
using System.Globalization;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();

            var result = GetLatestProjects(context);

            Console.WriteLine(result.ToString());
        }
        //Problem 3
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var result = context.Employees
                .OrderBy(e => e.EmployeeId)
                .ToList();

            foreach (var empl in result)
            {
                sb.AppendLine($"{empl.FirstName} {empl.LastName} {empl.MiddleName} {empl.JobTitle} {empl.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 4
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var result = context.Employees
                .Where(e => e.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();

            foreach (var employee in result)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            return sb.ToString().TrimEnd();

        }
        //Problem 5
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var result = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    DepartmentName = e.Department.Name,
                    Salary = e.Salary
                })
                .ToList();

            foreach (var empl in result)
            {
                sb.AppendLine($"{empl.FirstName} {empl.LastName} from Research and Development - ${empl.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 6
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            Address address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var employee = context.Employees
                .First(e => e.LastName == "Nakov");

            employee.Address = address;

            context.SaveChanges();


            List<string> result = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText)
                .ToList();

            foreach (var addr in result)
            {
                sb.AppendLine(addr);
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 7 
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var result = context.Employees
                .Where(e => e.EmployeesProjects.Any(e => e.Project.StartDate.Year >= 2001 && e.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Project = e.EmployeesProjects
                    .Select(ep => new
                    {
                        ProjectName = ep.Project.Name,
                        StartDate = ep.Project.StartDate
                    .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                        EndDate = ep.Project.EndDate.HasValue ? ep.Project.EndDate.Value
                    .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) :
                    "not finished"
                    })
                    .ToList()
                })
                .ToList();

            foreach (var empl in result)
            {
                sb.AppendLine($"{empl.FirstName} {empl.LastName} – - Manager: {empl.ManagerFirstName} {empl.ManagerLastName}");
                foreach (var p in empl.Project)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate} ");
                }
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 8
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var sb = new StringBuilder();
            var result = context
                .Addresses
                 .Select(a => new
                 {
                     a.AddressText,
                     a.Town.Name,
                     a.Employees.Count
                 })
                  .OrderByDescending(a => a.Count)
                    .ThenBy(a => a.Name)
                    .ThenBy(a => a.AddressText)
                    .Take(10)
                    .ToList();
            foreach (var a in result)
            {

                sb.AppendLine($"{a.AddressText}, {a.Name} - {a.Count} employees");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 9
        public static string GetEmployee147(SoftUniContext context)
        {

            var employeeInfo = context
               .Employees
               .Where(x => x.EmployeeId == 147)
               .Select(x => new
               {
                   x.FirstName,
                   x.LastName,
                   x.JobTitle,
                   Projects = x.EmployeesProjects
                     .Select(p => new { p.Project.Name })
                     .OrderBy(p => p.Name)
                     .ToList()
               })
               .First();

            var result = new StringBuilder();

            result.AppendLine($"{employeeInfo.FirstName} {employeeInfo.LastName} - {employeeInfo.JobTitle}");

            foreach (var project in employeeInfo.Projects)
            {
                result.AppendLine(project.Name);
            }

            return result.ToString().Trim();
        }
        //Problem 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var result = context.Departments
                .Where(d => d.Employees.Count() > 5)
                .OrderBy(d => d.Employees.Count())
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    DepartmentName = d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    })
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToList()
                })
                .ToList();

            foreach (var d in result)
            {
                sb.AppendLine($"{d.DepartmentName} - {d.ManagerFirstName} {d.ManagerLastName}");
                foreach (var e in d.Employees)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var result = context.Projects
                .OrderByDescending(u => u.StartDate).Take(10)
                .OrderBy(p => p.Name)
                .Select(x => new
                {
                    x.Name,
                    x.Description,
                    x.StartDate
                })
                .ToList();

            foreach (var p in result)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate.ToString("M/d/yyyy h:mm:ss tt"));
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            decimal raising = 1.12M;

            var employeesInfo = context
                .Employees
                .Where(x => x.Department.Name == "Engineering" ||
                x.Department.Name == "Tool Design" ||
                x.Department.Name == "Marketing" ||
                x.Department.Name == "Information Services").ToList();

            foreach (var e in employeesInfo)
            {
                e.Salary *= raising;
            }

            context.SaveChanges();

            var employees = context
                .Employees
                .Where(x => x.Department.Name == "Engineering" ||
                x.Department.Name == "Tool Design" ||
                x.Department.Name == "Marketing" ||
                x.Department.Name == "Information Services")
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Salary
                })
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:F2})");
            }

            return sb.ToString().Trim();
        }
        //Problem 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {

            var employees = context.Employees
                            .Where(e => e.FirstName.StartsWith("Sa") || e.FirstName.StartsWith("SA"))
                            .Select(e => new
                            {
                                e.FirstName,
                                e.LastName,
                                e.JobTitle,
                                e.Salary
                            }).ToList();

            var sb = new StringBuilder();

            foreach (var e in employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName))
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }
        //Problem 14
        public static string DeleteProjectById(SoftUniContext context)
        {
            var deletingIdFromEmployeeProjects = context
               .EmployeesProjects
               .Where(x => x.ProjectId == 2)
               .ToList();

            context.EmployeesProjects.RemoveRange(deletingIdFromEmployeeProjects);

            var deletingIdFromProjects = context
                .Projects
                .Where(x => x.ProjectId == 2)
                .FirstOrDefault();

            context.Projects.RemoveRange(deletingIdFromProjects);

            var taking10Projects = context
                .Projects
                .Take(10)
                .Select(x => new
                {
                    x.Name
                }).ToList();

            var sb = new StringBuilder();

            foreach (var p in taking10Projects)
            {
                sb.AppendLine(p.Name);
            }

            return sb.ToString().Trim();
        }
        //Problem 15
        public static string RemoveTown(SoftUniContext context)
        {
            var townName = "Seattle";

            var employeeInfo = context
                .Employees
                .Where(x => x.Address.Town.Name == townName)
                .ToList();

            foreach (var e in employeeInfo)
            {
                e.AddressId = null;
            }

            var addressesCount = context
                .Addresses
                .Where(x => x.Town.Name == townName)
                .Count();

            var addressesRemove = context
                .Addresses
                .Where(x => x.Town.Name == townName)
                .ToList();

            context.Addresses.RemoveRange(addressesRemove);

            var townRemove = context
               .Towns
               .Where(x => x.Name == townName)
               .ToList();

            context.Towns.RemoveRange(townRemove);

            context.SaveChanges();

            return $"{addressesCount} addresses in {townName} were deleted";
        }
    }
}
