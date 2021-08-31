using CarShop.Data;
using CarShop.Services;
using Microsoft.EntityFrameworkCore;

using MyWebServer;
using MyWebServer.Controllers;
using MyWebServer.Results.Views;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarShop
{
    public class Startup
    {
        public static async Task Main()
           => await HttpServer
               .WithRoutes(routes => routes
                   .MapStaticFiles()
                   .MapControllers())
               .WithServices(services => services
                   .Add<ApplicationDbContext>()
                   .Add<IViewEngine, CompilationViewEngine>()
                   .Add<IValidator, Validator>()
                   .Add<IUserService, UserService>()
                   .Add<IPasswordHasher, PasswordHasher>())
               .WithConfiguration<ApplicationDbContext>(context => context
                   .Database.Migrate())
               .Start();
    }
}
