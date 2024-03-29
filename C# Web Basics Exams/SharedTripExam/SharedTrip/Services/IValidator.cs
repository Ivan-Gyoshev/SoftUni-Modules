﻿namespace SharedTrip.Services
{
    using SharedTrip.Models.Trips;
    using SharedTrip.Models.Users;
    using System.Collections.Generic;


    public interface IValidator
    {
        ICollection<string> ValidateUser(UserRegisterModel model);
        ICollection<string> ValidateTrip(CreateTripFormModel model);
    }
}
