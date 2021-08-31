namespace Git.Controllers
{
    using Git.Data;
    using Git.Data.Models;
    using Git.Models.Repos;
    using Git.Services;
    using MyWebServer.Controllers;
    using MyWebServer.Http;
    using System.Linq;

    using static Data.DataConstants;

    public class RepositoriesController : Controller
    {
        private readonly IValidator validator;
        private readonly GitDbContext data;
        public RepositoriesController(IValidator validator, GitDbContext data)
        {
            this.validator = validator;
            this.data = data;
        }

        public HttpResponse All()
        {
            var repositories = this.data.Repositories
                .Where(r => r.IsPublic)
                .Select(r => new RepoListingModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Owner = r.Owner.Username,
                    CreatedOn = r.CreatedOn.ToLocalTime().ToString("F"),
                    Commits = r.Commits.Count()
                })
                .ToList();

            return View(repositories);
        }

        [Authorize]
        public HttpResponse Create() => View();

        [Authorize]
        [HttpPost]
        public HttpResponse Create(RepoCreateModel model)
        {
            var errors = this.validator.ValidateRepository(model);

            if (errors.Any())
            {
                return Error(errors);
            }

            var repository = new Repository
            {
                Name = model.Name,
                IsPublic = model.RepositoryType == RepositoryPublicType,
                OwnerId = this.User.Id
            };

            this.data.Repositories.Add(repository);
            this.data.SaveChanges();

            return Redirect("/Repositories/All");
        }
    }
}
