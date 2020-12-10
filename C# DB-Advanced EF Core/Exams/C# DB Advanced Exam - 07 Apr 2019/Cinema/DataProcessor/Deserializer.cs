namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";
        private const string SuccessfulImportMovie
            = "Successfully imported {0} with genre {1} and rating {2}!";
        private const string SuccessfulImportHallSeat
            = "Successfully imported {0}({1}) with {2} seats!";
        private const string SuccessfulImportProjection
            = "Successfully imported projection {0} on {1}!";
        private const string SuccessfulImportCustomerTicket
            = "Successfully imported customer {0} {1} with bought tickets: {2}!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var movies = new List<Movie>();

            var movieDtos = JsonConvert.DeserializeObject<ImportMovieDto[]>(jsonString);

            foreach (var dto in movieDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var movie = movies.FirstOrDefault(m => m.Title == dto.Title);
                if (movie != null)
                {
                    if (movies.Contains(movie))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                }

                var newMovie = new Movie
                {
                    Title = dto.Title,
                    Genre = dto.Genre,
                    Rating = dto.Rating,
                    Director = dto.Director
                };
                movies.Add(newMovie);
                sb.AppendLine(string.Format(SuccessfulImportMovie, newMovie.Title, newMovie.Genre.ToString(), newMovie.Rating.ToString("f2")));
            }
            context.AddRange(movies);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var halls = new List<Hall>();

            var hallDtos = JsonConvert.DeserializeObject<ImportHallSeatsDto[]>(jsonString);

            foreach (var dto in hallDtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var hall = new Hall()
                {
                    Name = dto.Name,
                    Is4Dx = dto.Is4Dx,
                    Is3D = dto.Is3D
                };

                for (int i = 0; i < dto.Seats; i++)
                {
                    var seat = new Seat();
                    hall.Seats.Add(seat);
                }

                if (hall.Seats.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                halls.Add(hall);
                var type = GetProjectType(hall);

                sb.AppendLine(string.Format(SuccessfulImportHallSeat, hall.Name, type.ToString(), hall.Seats.Count));
            }
            context.Halls.AddRange(halls);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static object GetProjectType(Hall hall)
        {
            var projType = "Normal";
            if (hall.Is3D && hall.Is4Dx)
            {
                projType = "4Dx/3D";
            }
            else if (hall.Is3D)
            {
                projType = "3D";
            }
            else if (hall.Is4Dx)
            {
                projType = "4Dx";
            }
            return projType;
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var projections = new List<Projection>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProjectionsDto[]), new XmlRootAttribute("Projections"));

            using (StringReader stringReader = new StringReader(xmlString))
            {
                var projectionDtos = (ImportProjectionsDto[])xmlSerializer.Deserialize(stringReader);


                foreach (var dto in projectionDtos)
                {
                    if (!IsValid(dto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var movie = context.Movies.Find(dto.MovieId);
                    var hall = context.Halls.Find(dto.HallId);

                    if (movie == null || hall == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime datetime;
                    bool isDateValid = DateTime.TryParseExact(dto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out datetime);
                    if (!isDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var projection = new Projection
                    {
                        MovieId = dto.MovieId,
                        HallId = dto.HallId,
                        DateTime = datetime
                    };
                    var date = projection.DateTime.ToString("MM\\/dd\\/yyyy");
                    projections.Add(projection);
                    sb.AppendLine(string.Format(SuccessfulImportProjection, movie.Title, date));
                }
                context.Projections.AddRange(projections);
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var customers = new List<Customer>();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomersDto[]), new XmlRootAttribute("Customers"));

            using (StringReader stringReader = new StringReader(xmlString))
            {
                var customerDtos = (ImportCustomersDto[])xmlSerializer.Deserialize(stringReader);

                foreach (var dto in customerDtos)
                {
                    if (!IsValid(dto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var customer = new Customer
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Age = dto.Age,
                        Balance = dto.Balance
                    };
                    foreach (var ticketDto in dto.Tickets)
                    {
                        if (!IsValid(ticketDto))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        var ticket = new Ticket()
                        {
                            ProjectionId = ticketDto.ProjectionId,
                            Price = ticketDto.Price
                        };
                        customer.Tickets.Add(ticket);
                    }
                    customers.Add(customer);
                    sb.AppendLine(string.Format(SuccessfulImportCustomerTicket, customer.FirstName, customer.LastName, customer.Tickets.Count));
                }
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}