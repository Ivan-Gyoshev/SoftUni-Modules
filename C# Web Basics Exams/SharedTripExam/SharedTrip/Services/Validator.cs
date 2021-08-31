namespace SharedTrip.Services
{
    using SharedTrip.Models.Trips;
    using SharedTrip.Models.Users;
    using System.Collections.Generic;
    using System.Linq;
    using static Data.DataConstants;


    public class Validator : IValidator
    {
        public ICollection<string> ValidateUser(UserRegisterModel user)
        {
            var errors = new List<string>();

            if (user.Username == null || user.Username.Length < UserMinUsername || user.Username.Length > DefaultMaxLength)
            {
                errors.Add($"Username '{user.Username}' is not valid. It must be between {UserMinUsername} and {DefaultMaxLength} characters long.");
            }

            if (user.Password == null || user.Password.Length < UserMinPassword || user.Password.Length > DefaultMaxLength)
            {
                errors.Add($"The provided password is not valid. It must be between {UserMinPassword} and {DefaultMaxLength} characters long.");
            }

            if (user.Password != null && user.Password.Any(x => x == ' '))
            {
                errors.Add($"The provided password cannot contain whitespaces.");
            }

            if (user.Password != user.ConfirmPassword)
            {
                errors.Add("Password and its confirmation are different.");
            }

            return errors;
        }


        public ICollection<string> ValidateTrip(CreateTripFormModel model)
        {
            var errors = new List<string>();

            if (model.Seats < SeatsMin || model.Seats > SeatsMax)
            {
                errors.Add("Seats should be between 2 and 6.");
            }

            if (model.Description.Length > DescriptionMax)
            {
                errors.Add("Description can not be longer than 80 symbols.");
            }

            return errors;
        }
    }
}
