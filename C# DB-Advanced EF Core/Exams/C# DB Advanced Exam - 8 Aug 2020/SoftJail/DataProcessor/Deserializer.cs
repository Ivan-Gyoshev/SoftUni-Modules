namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var departments = new List<Department>();

            var departmentDtos = JsonConvert.DeserializeObject<ImportDepartmentDto[]>(jsonString);

            foreach (var dto in departmentDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                if (dto.Cells.Length == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                var department = new Department
                {
                    Name = dto.Name
                };

                foreach (var cellDto in dto.Cells)
                {
                    if (!IsValid(cellDto))
                    {
                        break;
                    }

                    department.Cells.Add(new Cell
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    });
                }
                if (department.Cells.Count == 0)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
                departments.Add(department);
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }
            context.Departments.AddRange(departments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var prisoners = new List<Prisoner>();

            var prisonerDtos = JsonConvert.DeserializeObject<ImportPrisonerDto[]>(jsonString);

            foreach (var dto in prisonerDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                DateTime incarceration;
                bool isIncarDateValid = DateTime.TryParseExact(dto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out incarceration);

                if (!isIncarDateValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                DateTime? releaseDate;
                if (!string.IsNullOrEmpty(dto.ReleaseDate))
                {
                    DateTime dateValue;
                    bool isDateValueValid = DateTime.TryParseExact(dto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue);

                    if (!isDateValueValid)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }
                    releaseDate = dateValue;
                }
                else
                {
                    releaseDate = null;
                }

                var prisoner = new Prisoner
                {
                    FullName = dto.FullName,
                    Nickname = dto.Nickname,
                    Age = dto.Age,
                    IncarcerationDate = incarceration,
                    ReleaseDate = releaseDate,
                    CellId = dto.CellId,
                };

                foreach (var mailDto in dto.Mails)
                {
                    if (!IsValid(mailDto))
                    {
                        continue;
                    }
                    prisoner.Mails.Add(new Mail
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address
                    });
                }
                prisoners.Add(prisoner);
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }
            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var officers = new List<Officer>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportOfficerDto[]), new XmlRootAttribute("Officers"));
            


            using (StringReader stringReader = new StringReader(xmlString))
            {
                var officerDtos = (ImportOfficerDto[])xmlSerializer.Deserialize(stringReader);

                foreach (var dto in officerDtos)
                {
                    if (!IsValid(dto))
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }
                    Object posRes;
                    bool isEnumValid = Enum.TryParse(typeof(Position), dto.Position, out posRes);
                    if (!isEnumValid)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    Object wepRes;
                    bool isEnumValid2 = Enum.TryParse(typeof(Weapon), dto.Weapon, out wepRes);
                    if (!isEnumValid2)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }
                    var officer = new Officer
                    {
                        FullName = dto.FullName,
                        Salary = dto.Salary,
                        Position = (Position)posRes,
                        Weapon = (Weapon)wepRes,
                        DepartmentId = dto.DepartmentId
                    };
                    foreach (var prisonerDtos in dto.Prisoners)
                    {
                        var prisoner = new Prisoner
                        {
                            Id = prisonerDtos.PrisonerId
                        };


                        officer.OfficerPrisoners.Add(new OfficerPrisoner
                        {
                            Officer = officer,
                            Prisoner = prisoner
                        });
                    }
                    officers.Add(officer);
                    sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
                }
                context.Officers.AddRange(officers);
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}