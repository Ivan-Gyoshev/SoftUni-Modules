namespace Git.Controllers
{
    using Git.Data;
    using Git.Data.Models;
    using Git.Models.Commits;
    using Git.Services;
    using MyWebServer.Controllers;
    using MyWebServer.Http;
    using System.Linq;

    using static Data.DataConstants;

    public class CommitsController : Controller
    {
        private readonly IValidator validator;
        private readonly GitDbContext data;
        public CommitsController(IValidator validator, GitDbContext data)
        {
            this.validator = validator;
            this.data = data;
        }

        [Authorize]
        public HttpResponse Create(string id)
        {
            var repo = this.data.Repositories
                .Where(r => r.Id == id)
                .Select(r => new CommitToRepositoryModel
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .FirstOrDefault();

            if (repo == null)
            {
                return BadRequest();
            }

            return View(repo);
        }
        [Authorize]
        [HttpPost]
        public HttpResponse Create(CreateCommitModel model)
        {
            if (!this.data.Repositories.Any(r => r.Id == model.Id))
            {
                return Error("Repository does not exist.");
            }

            if (model.Description.Length < DescriptionMinLength)
            {
                return Error($"Description must be atleast {DescriptionMinLength} symbols long.");
            }

            var commit = new Commit
            {
                Description = model.Description,
                CreatorId = this.User.Id,
                RepositoryId = model.Id
            };

            this.data.Commits.Add(commit);
            this.data.SaveChanges();

            return Redirect("/Commits/All");
        }
        [Authorize]
        public HttpResponse All()
        {
            var commits = this.data
               .Commits
               .Where(c => c.CreatorId == this.User.Id)
               .OrderByDescending(c => c.CreatedOn)
               .Select(c => new CommitsListingModel
               {
                   Id = c.Id,
                   Description = c.Description,
                   CreatedOn = c.CreatedOn.ToLocalTime().ToString("F"),
                   Repository = c.Repository.Name
               })
               .ToList();

            return View(commits);
        }

        [Authorize]
        public HttpResponse Delete(string id)
        {
            bool isOwner = this.data
                .Commits
                .Any(c => c.CreatorId == this.User.Id);

            if (!isOwner)
            {
                return Unauthorized();
            }

            var commit = this.data.Commits
                .Where(c => c.Id == id)
                .FirstOrDefault();

            this.data.Remove(commit);
            this.data.SaveChanges();

            return Redirect("/Commits/All");
        }
    }
}
