namespace BattleCards.Controllers
{
    using BattleCards.Data;
    using BattleCards.Data.Models;
    using BattleCards.Models.Cards;
    using BattleCards.Services;
    using MyWebServer.Controllers;
    using MyWebServer.Http;
    using System.Linq;

    public class CardsController : Controller
    {
        private readonly IValidator validator;
        private readonly ApplicationDbContext data;

        public CardsController(IValidator validator, ApplicationDbContext data)
        {
            this.validator = validator;
            this.data = data;
        }

        [Authorize]
        public HttpResponse All()
        {
            var cards = this.data.Cards
                .Select(c => new CardsListingModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Image = c.ImageUrl,
                    Description = c.Description,
                    Keyword = c.Keyword,
                    Attack = c.Attack,
                    Health = c.Health
                })
                .ToList();

            return View(cards);
        }

        [Authorize]
        public HttpResponse Collection()
        {

            var userCards = this.data.UserCards
                .Where(u => u.UserId == this.User.Id)
                .Select(c => new CardsListingModel
                {
                    Id = c.CardId,
                    Name = c.Card.Name,
                    Image = c.Card.ImageUrl,
                    Description = c.Card.Description,
                    Keyword = c.Card.Keyword,
                    Attack = c.Card.Attack,
                    Health = c.Card.Health
                })
                .ToList();

            return View(userCards);
        }

        [Authorize]
        public HttpResponse Add() => View();

        [Authorize]
        [HttpPost]
        public HttpResponse Add(CardAddModel model)
        {
            var errors = this.validator.ValidateCard(model);

            if (errors.Any())
            {
                return Error(errors);
            }

            var card = new Card
            {
                Name = model.Name,
                ImageUrl = model.Image,
                Keyword = model.Keyword,
                Description = model.Description,
                Attack = model.Attack,
                Health = model.Health,
            };

            this.data.Cards.Add(card);

            var cardId = card.Id.ToString();

            var userCard = new UserCard
            {
                UserId = this.User.Id,
                CardId = int.Parse(cardId)
            };

            this.data.UserCards.Add(userCard);

            this.data.SaveChanges();

            return Redirect("/Cards/All");
        }
    }
}
