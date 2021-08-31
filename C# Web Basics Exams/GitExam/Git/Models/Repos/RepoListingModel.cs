namespace Git.Models.Repos
{
    public class RepoListingModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Owner { get; set; }

        public string CreatedOn { get; set; }

        public int Commits { get; set; }
    }
}
