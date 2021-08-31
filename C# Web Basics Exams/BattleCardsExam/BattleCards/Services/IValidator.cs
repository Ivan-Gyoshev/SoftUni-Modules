namespace BattleCards.Services
{
    using BattleCards.Models;
    using BattleCards.Models.Cards;
    using System.Collections.Generic;

    public interface IValidator
    {
        ICollection<string> ValidateUser(UserCreateModel user);
        ICollection<string> ValidateCard(CardAddModel card);
    }
}
