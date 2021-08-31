namespace CarShop.Controllers
{
    using CarShop.Data;
    using CarShop.Data.Models;
    using CarShop.Services;
    using CarShop.ViewModels.Cars;
    using MyWebServer.Controllers;
    using MyWebServer.Http;
    using System.Collections.Generic;
    using System.Linq;

    public class CarsController : Controller
    {
        private readonly IValidator validator;
        private readonly IUserService userService;
        private readonly ApplicationDbContext data;

        public CarsController(IValidator validator, ApplicationDbContext data, IUserService userService)
        {
            this.validator = validator;
            this.data = data;
            this.userService = userService;
        }

        //Get-AddCars page
        public HttpResponse Add()
        {
            if (this.userService.IsMechanic(this.User.Id))
            {
                return Unauthorized();
            }

            return View();
        }

        //Post-AddCars
        [Authorize]
        [HttpPost]
        public HttpResponse Add(CarCreateModel model)
        {
            if (this.userService.IsMechanic(this.User.Id))
            {
                return Unauthorized();
            }

            var modelErrors = this.validator.ValidateCars(model);

            if (modelErrors.Any())
            {
                return Error(modelErrors);
            }

            var car = new Car
            {
                Model = model.Model,
                Year = model.Year,
                PictureUrl = model.Image,
                PlateNumber = model.PlateNumber,
                OwnerId = this.User.Id
            };

            this.data.Cars.Add(car);
            this.data.SaveChanges();

            return Redirect("/Cars/All");
        }

        [Authorize]
        public HttpResponse All()
        {
            var carsQuery = this.data.Cars.AsQueryable();

            if (this.userService.IsMechanic(this.User.Id))
            {
                carsQuery = carsQuery
                    .Where(c => c.Issues.Any(i => !i.IsFixed));
            }
            else
            {
                carsQuery = carsQuery
                   .Where(c => c.OwnerId == this.User.Id);
            }

            var cars = carsQuery
               .Select(c => new CarsAllModel
               {
                   Id = c.Id,
                   Model = c.Model,
                   Year = c.Year,
                   Image = c.PictureUrl,
                   PlateNumber = c.PlateNumber,
                   FixedIssues = c.Issues.Where(i => i.IsFixed).Count(),
                   RemainingIssues = c.Issues.Where(i => !i.IsFixed).Count()
               })
               .ToList();

            return View(cars);
        }
    }
}
