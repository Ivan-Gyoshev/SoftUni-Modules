namespace BattleCards.Services
{
    using BattleCards.Models;
    using BattleCards.Models.Cards;
    using System.Collections.Generic;

    using static Data.DataConstants;

    public class Validator : IValidator
    {
        public ICollection<string> ValidateUser(UserCreateModel user)
        {
            var errors = new List<string>();

            if (user.Username.Length < UserMinUsername || user.Username.Length > DefaultMaxLength)
            {
                errors.Add("Username must be between 5 and 20 symbols long.");
            }

            if (user.Password.Length < UserMinPassword || user.Password.Length > DefaultMaxLength)
            {
                errors.Add("Password must be between 6 and 20 symbols long.");
            }

            if (user.Password != user.ConfirmPassword)
            {
                errors.Add("Password and ConfirmPassword do not match!");
            }

            return errors;
        }

        public ICollection<string> ValidateCard(CardAddModel card)
        {
            var errors = new List<string>();

            if (card.Name.Length < CardMinName || card.Name.Length > DefaultMaxLength)
            {
                errors.Add("Card name must be between 5 and 20 symbols long.");
            }

            if (card.Description.Length > CardDescriptionMax)
            {
                errors.Add("Description maximum length is 200 symbols.");
            }

            if (card.Health < 0)
            {
                errors.Add("Card health cannot be below zero.");
            }
            
            if (card.Attack < 0)
            {
                errors.Add("Card attack cannot be below zero.");
            }

            return errors;
        }
    }
}
