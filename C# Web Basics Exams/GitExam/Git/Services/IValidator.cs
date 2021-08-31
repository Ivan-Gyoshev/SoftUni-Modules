namespace Git.Services
{
    using Git.Models.Repos;
    using Git.Models.Users;
    using System.Collections.Generic;

    public interface IValidator
    {
        ICollection<string> ValidateUser(UserCreateModel user);
        ICollection<string> ValidateRepository(RepoCreateModel repo);
    }
}
