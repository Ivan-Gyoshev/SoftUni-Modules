namespace CarShop.ViewModels.Issues
{
    using System.Collections.Generic;

    public class CarIssueModel
    {
        public string Id { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public IEnumerable<IssueAllModel> Issues { get; set; }
    }
}
