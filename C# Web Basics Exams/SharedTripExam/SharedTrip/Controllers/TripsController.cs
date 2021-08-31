namespace SharedTrip.Controllers
{
    using MyWebServer.Controllers;
    using MyWebServer.Http;
    using SharedTrip.Data;
    using SharedTrip.Data.Models;
    using SharedTrip.Models.Trips;
    using SharedTrip.Services;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using static Data.DataConstants;

    public class TripsController : Controller
    {
        private readonly IValidator validator;
        private readonly ApplicationDbContext data;

        public TripsController(IValidator validator, ApplicationDbContext data)
        {
            this.validator = validator;
            this.data = data;
        }

        [Authorize]
        public HttpResponse All()
        {
            var trips = this.data.Trips
                .Select(t => new TripListingViewModel
                {
                    Id = t.Id,
                    StartPoint = t.StartPoint,
                    EndPoint = t.EndPoint,
                    DepartureTime = t.DepartureTime.ToString("dd.MM.yyyy HH:mm"),
                    Seats = t.Seats,
                })
                .ToList();


            return View(trips);
        }

        [Authorize]
        public HttpResponse Add() => View();

        [Authorize]
        [HttpPost]
        public HttpResponse Add(CreateTripFormModel model)
        {
            var modelErrors = this.validator.ValidateTrip(model);

            if (modelErrors.Any())
            {
                return Redirect("/Trips/Add");
            }

            DateTime departure;
            bool isIncarDateValid = DateTime.TryParseExact(model.DepartureTime, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out departure);

            var trip = new Trip
            {
                StartPoint = model.StartPoint,
                EndPoint = model.EndPoint,
                DepartureTime = departure,
                ImagePath = model.ImagePath,
                Seats = model.Seats,
                Description = model.Description
            };

            trip.UserTrips.Add(new UserTrip
            {
                UserId = this.User.Id,
            });

            this.data.Trips.Add(trip);
            this.data.SaveChanges();

            return Redirect("/Trips/All");
        }

        [Authorize]
        public HttpResponse Details(string tripId)
        {
            var trip = this.data.Trips
                .Where(t => t.Id == tripId)
                .Select(t => new TripDetailsViewModel
                {
                    Id = t.Id,
                    Image = t.ImagePath,
                    StartPoint = t.StartPoint,
                    EndPoint = t.EndPoint,
                    DepartureTime = t.DepartureTime.ToString("dd.MM.yyyy HH:mm"),
                    Seats = t.Seats,
                    Description = t.Description
                })
                .FirstOrDefault();

            return View(trip);
        }

        [Authorize]
        public HttpResponse AddUserToTrip(string tripId)
        {
            try
            {
                var errors = new List<string>();

                var trip = this.data.Trips
                    .Where(t => t.Id == tripId)
                    .FirstOrDefault();

                if (trip.Seats == 2)
                {
                    return Redirect($"/Trips/Details?tripId={tripId}");
                }
                if (trip.UserTrips.Any(ut => ut.UserId == this.User.Id))
                {
                    return Redirect($"/Trips/Details?tripId={tripId}");
                }

                else
                {
                    trip.UserTrips.Add(new UserTrip
                    {
                        UserId = this.User.Id
                    });

                    trip.Seats--;

                    this.data.SaveChanges();
                }

                return Redirect("/Trips/All");
            }
            catch (Exception)
            {

                return Redirect($"/Trips/Details?tripId={tripId}");
            }
        }
    }
}
