namespace Git.Models.Commits
{
    public class CommitsListingModel
    {
        public string Id { get; set; }

        public string Repository { get; set; }

        public string CreatedOn { get; set; }

        public string Description { get; set; }
    }
}
