namespace TeisterMask.DataProcessor
{
    using System;
    using System.Text;
    using System.Linq;
    using System.Globalization;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;
    using Newtonsoft.Json;

    using Data;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using System.Xml.Serialization;
    using System.IO;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var projects = new List<Project>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProjectDto[]), new XmlRootAttribute("Projects"));

            using (StringReader stringReader = new StringReader(xmlString))
            {
                var projectDtos = (ProjectDto[])xmlSerializer.Deserialize(stringReader);

                foreach (var dto in projectDtos)
                {
                    if (!IsValid(dto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                   // Validate Date //

                    DateTime projectOpenDate;
                    bool isProjectOpenDateValid = DateTime.TryParseExact(dto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None,out projectOpenDate);

                    if (!isProjectOpenDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime? projectDueDate;
                    if (!string.IsNullOrEmpty(dto.DueDate))
                    {
                        DateTime projectDueDateValue;
                        bool isProjectDueDateValid = DateTime.TryParseExact(dto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out projectDueDateValue);

                        if (!isProjectDueDateValid)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                        projectDueDate = projectDueDateValue;
                    }
                    else
                    {
                        projectDueDate = null;
                    }

                    var project = new Project
                    {
                        Name = dto.Name,
                        OpenDate = projectOpenDate,
                        DueDate = projectDueDate
                    };

                    foreach (var t in dto.Tasks)
                    {
                        if (!IsValid(t))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        DateTime taskOpenDate;
                        bool isTaskOpenDateValid = DateTime.TryParseExact(t.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out taskOpenDate);
                        if (!isTaskOpenDateValid)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        DateTime taskDueDate;
                        bool isTaskDueDateValid = DateTime.TryParseExact(t.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out taskDueDate);
                        if (!isTaskDueDateValid)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (taskOpenDate < projectOpenDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                        if (projectDueDate.HasValue)
                        {
                            if (taskDueDate > projectDueDate.Value)
                            {
                                sb.AppendLine(ErrorMessage);
                                continue;
                            }
                        }

                        project.Tasks.Add(new Task()
                        { 
                        Name = t.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)t.ExecutionType,
                        LabelType = (LabelType)t.LabelType
                        });

                    }

                    projects.Add(project);
                    sb.AppendLine(string.Format(SuccessfullyImportedProject,project.Name,project.Tasks.Count));
                }

                context.Projects.AddRange(projects);
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }
        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var employees = new List<Employee>();
            var employeesTasksForImport = new List<EmployeeTask>();
            var employeesDto = JsonConvert.DeserializeObject<EmployeeDto[]>(jsonString);

            var tasksId = context.Tasks.Select(i => i.Id).ToList();

            foreach (var dto in employeesDto)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!IsUsernameValid(dto.Username))
                {
                    sb.AppendLine(ErrorMessage);
                }
                var employee = new Employee()
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Phone = dto.Phone
                };

                var curentTasks = new List<EmployeeTask>();

                foreach (var t in dto.Tasks.Distinct())
                {
                    if (!tasksId.Contains(t))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var newEmployeeTask = new EmployeeTask
                    {
                        Employee = employee,
                        TaskId = t
                    };

                    curentTasks.Add(newEmployeeTask);

                }

                employeesTasksForImport.AddRange(curentTasks);
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, curentTasks.Count));
            }
            context.Employees.AddRange(employees);
            context.EmployeesTasks.AddRange(employeesTasksForImport);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        private static bool IsUsernameValid(string username)
        {
            foreach (var ch in username)
            {
                if (!Char.IsLetterOrDigit(ch))
                {
                    return false;
                }
            }
            return true;
        }


        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}