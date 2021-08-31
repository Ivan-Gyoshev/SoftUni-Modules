namespace CarShop.Controllers
{
    using CarShop.Data;
    using CarShop.Data.Models;
    using CarShop.Services;
    using CarShop.ViewModels.Issues;
    using MyWebServer.Controllers;
    using MyWebServer.Http;
    using System.Linq;

    public class IssuesController : Controller
    {
        private readonly IUserService userService;
        private readonly ApplicationDbContext data;

        public IssuesController(IUserService userService, ApplicationDbContext data)
        {
            this.userService = userService;
            this.data = data;
        }

        [Authorize]
        public HttpResponse CarIssues(string carId)
        {
            if (!this.userService.IsMechanic(this.User.Id))
            {
                var userOwnerCar = this.data.Cars
                    .Any(c => c.Id == carId && c.OwnerId == this.User.Id);

                if (!userOwnerCar)
                {
                    return Error("You do not have access to this car.");
                }
            }

            var carWithIssue = this.data
                .Cars
                .Where(c => c.Id == carId)
                .Select(c => new CarIssueModel
                {
                    Id = c.Id,
                    Model = c.Model,
                    Year = c.Year,
                    Issues = c.Issues.Select(i => new IssueAllModel
                    {
                        Id = i.Id,
                        Description = i.Description,
                        IsFixed = i.IsFixed
                    })
                })
                .FirstOrDefault();

            if (carWithIssue == null)
            {
                return Error("This car does not exist.");
            }

            return View(carWithIssue);
        }

        public HttpResponse Add() => View();

        [Authorize]
        [HttpPost]
        public HttpResponse Add(string carId,string description)
        {
            if (description.Length < 5)
            {
                return Error("The description is not valid!");
            }

            var issue = new Issue
            {
                CarId = carId,
                Description = description
            };

            this.data.Issues.Add(issue);
            this.data.SaveChanges();

            return this.View();
        }

        public HttpResponse Delete(string issueId, string carId)
        {
            if (!this.userService.IsMechanic(this.User.Id))
            {
                return Error("You are not mechanic!");
            }

            var currentIssue = this.data.Issues
                .FirstOrDefault(i => i.Id == issueId && i.CarId == carId);

            this.data.Issues.Remove(currentIssue);
            this.data.SaveChanges();

            return View($"/Issues/CarIssues?carId={carId}");
        }
    }
}
