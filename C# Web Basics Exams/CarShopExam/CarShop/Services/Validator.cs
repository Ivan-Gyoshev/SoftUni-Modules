using CarShop.ViewModels.Cars;
using CarShop.ViewModels.Users;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using static CarShop.Data.DataConstants;

namespace CarShop.Services
{
    public class Validator : IValidator
    {
        public ICollection<string> ValidateUser(UserCreateModel user)
        {
            var errors = new List<string>();

            if (user.Username.Length < 4 || user.Username.Length > 20)
            {
                errors.Add("Username must be between 4 and 20 symbols long.");
            }

            if (user.Password.Length < 5 || user.Password.Length > 20)
            {
                errors.Add("Password must be between 5 and 20 symbols long.");
            }

            if (user.Password != user.ConfirmPassword)
            {
                errors.Add("Password and ConfirmPassword do not match!");
            }

            if (user.UserType != "Mechanic" && user.UserType != "Client")
            {
                errors.Add("User Type must be either 'Mechanic' or 'Client' ");
            }

            return errors;
        }
        public ICollection<string> ValidateCars(CarCreateModel car)
        {
            var errors = new List<string>();

            if (car.Model.Length < 5 || car.Model.Length > 20)
            {
                errors.Add("Car model must be between 5 and 20 symbols long.");
            }
            if (car.Year < 1900 || car.Year > 2100)
            {
                errors.Add("Year is not valid!");
            }
            if (!Regex.IsMatch(car.PlateNumber, CarPlateNumberRegularExpression))
            {
                errors.Add("Plate number is not valid!");
            }

            return errors;
        }
    }
}
