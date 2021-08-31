namespace BattleCards.Controllers
{
    using BattleCards.Data;
    using BattleCards.Data.Models;
    using BattleCards.Models;
    using BattleCards.Models.Users;
    using BattleCards.Services;
    using MyWebServer.Controllers;
    using MyWebServer.Http;
    using System.Linq;

    public class UsersController : Controller
    {
        private readonly IValidator validator;
        private readonly IPasswordHasher passwordHasher;
        private readonly ApplicationDbContext data;

        public UsersController(IValidator validator, ApplicationDbContext data, IPasswordHasher passwordHasher)
        {
            this.validator = validator;
            this.data = data;
            this.passwordHasher = passwordHasher;
        }

        //Register-Get
        public HttpResponse Register() => View();

        //Register-Post

        [HttpPost]
        public HttpResponse Register(UserCreateModel model)
        {
            var modelErrors = this.validator.ValidateUser(model);

            if (this.data.Users.Any(u => u.Username == model.Username))
            {
                modelErrors.Add("This username already exists!");
            }
            if (this.data.Users.Any(u => u.Email == model.Email))
            {
                modelErrors.Add("This email has already been used!");
            }

            if (modelErrors.Any())
            {
                return Error(modelErrors);
            }

            var user = new User
            {
                Username = model.Username,
                Password = this.passwordHasher.HashPassword(model.Password),
                Email = model.Email,
            };

            data.Users.Add(user);
            data.SaveChanges();

            return Redirect("/Users/Login");
        }

        //Login-Get
        public HttpResponse Login() => View();

        //Login-Post
        [HttpPost]
        public HttpResponse Login(UserLoginModel model)
        {
            var hashedPassword = this.passwordHasher.HashPassword(model.Password);
            var userId = this.data.Users
                .Where(u => u.Username == model.Username && u.Password == hashedPassword)
                .Select(u => u.Id)
                .FirstOrDefault();

            if (userId == null)
            {
                return Error("Username and password combination is not valid.");
            }

            this.SignIn(userId);

            return Redirect("/Cards/All");
        }

        //Logout
        public HttpResponse Logout()
        {
            this.SignOut();

            return Redirect("/");
        }
    }
}
