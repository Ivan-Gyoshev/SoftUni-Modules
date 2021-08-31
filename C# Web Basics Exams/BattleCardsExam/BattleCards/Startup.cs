namespace BattleCards
{
    using BattleCards.Data;
    using BattleCards.Services;
    using Microsoft.EntityFrameworkCore;
    using MyWebServer;
    using MyWebServer.Controllers;
    using MyWebServer.Results.Views;
    using System.Threading.Tasks;


    public class Startup
    {
        public static async Task Main()
           => await HttpServer
               .WithRoutes(routes => routes
                   .MapStaticFiles()
                   .MapControllers())
               .WithServices(services => services
                   .Add<IViewEngine, CompilationViewEngine>()
                   .Add<IValidator, Validator>()
                   .Add<IPasswordHasher, PasswordHasher>()
                   .Add<ApplicationDbContext>())
           .WithConfiguration<ApplicationDbContext>(context => context
                  .Database.Migrate())
               .Start();
    }
}
