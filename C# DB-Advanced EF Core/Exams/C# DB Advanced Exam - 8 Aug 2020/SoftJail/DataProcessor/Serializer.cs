namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var result = context.Prisoners
                 .Where(p => ids.Contains(p.Id))
                 .Select(p => new
                 {
                     Id = p.Id,
                     Name = p.FullName,
                     CellNumber = p.Cell.CellNumber,
                     Officers = p.PrisonerOfficers
                     .OrderBy(po=>po.Officer.FullName)
                     .Select(po => new
                     {
                         OfficerName = po.Officer.FullName,
                         Department = po.Officer.Department.Name
                     })
                     .ToArray(),
                     TotalOfficerSalary = double.Parse(p.PrisonerOfficers.Sum(po => po.Officer.Salary).ToString("f2"))
                 })
                 .OrderBy(p => p.Name)
                 .ThenBy(p => p.Id)
                 .ToArray();

            string json = JsonConvert.SerializeObject(result, Formatting.Indented);
            return json;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            throw new NotImplementedException();
        }
    }
}