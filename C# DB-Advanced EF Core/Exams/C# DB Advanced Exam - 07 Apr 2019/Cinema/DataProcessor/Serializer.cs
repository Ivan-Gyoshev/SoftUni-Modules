namespace Cinema.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Cinema.XMLHelper;
    using Data;
    using Microsoft.SqlServer.Server;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            var topMovies = context
                .Movies
                .ToArray()
                .OrderByDescending(m => m.Rating)
                .ThenByDescending(m => m.Projections.Sum(t => t.Tickets.Sum(p => p.Price)))
                .Where(m => m.Rating >= rating && m.Projections.Any(p => p.Tickets.Any()))
                .Select(m => new
                {
                    MovieName = m.Title,
                    Rating = m.Rating.ToString("f2"),
                    TotalIncomes = m.Projections.Sum(t => t.Tickets.Sum(p => p.Price)).ToString("F2"),
                    Customers = m.Projections
                   .SelectMany(t => t.Tickets)
                   .Select(c => new
                   {
                       FirstName = c.Customer.FirstName,
                       LastName = c.Customer.LastName,
                       Balance = c.Customer.Balance.ToString("f2")
                   })
                   .ToArray()
                   .OrderByDescending(c => c.Balance)
                   .ThenBy(c => c.FirstName)

                })
                .ToArray()
                .Take(10);

            string json = JsonConvert.SerializeObject(topMovies, Formatting.Indented);
            return json;
        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var targetCustomers = context.Customers
                .Where(x => x.Age >= age)
                .Select(x => new ExportCustomerDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SpentMoney = x.Tickets.Sum(t => t.Price).ToString("0.00"),
                    SpentTime = TimeSpan.FromSeconds(x.Tickets.Sum(t => t.Projection.Movie.Duration.TotalSeconds)).ToString(@"hh\:mm\:ss")

                })
                .OrderByDescending(x => decimal.Parse(x.SpentMoney))
                .Take(10)
                .ToList();

            var resultXml = XMLConverter.Serialize(targetCustomers, "Customers");
            return resultXml;
        }
    }
}