namespace Git.Services
{
    using Git.Models.Repos;
    using Git.Models.Users;
    using System.Collections.Generic;

    using static Data.DataConstants;

    public class Validator : IValidator
    {
        public ICollection<string> ValidateUser(UserCreateModel user)
        {
            var errors = new List<string>();

            if (user.Username.Length < UserMinUsername || user.Username.Length > DefaultMaxLength)
            {
                errors.Add("Username must be between 5 and 20 symbols long.");
            }

            if (user.Password.Length < UserMinPassword || user.Password.Length > DefaultMaxLength)
            {
                errors.Add("Password must be between 6 and 20 symbols long.");
            }

            if (user.Password != user.ConfirmPassword)
            {
                errors.Add("Password and ConfirmPassword do not match!");
            }

            return errors;
        }

        public ICollection<string> ValidateRepository(RepoCreateModel repo)
        {
            var errors = new List<string>();

            if (repo.Name.Length < RepoMinLength || repo.Name.Length > RepoMaxLength)
            {
                errors.Add("Repository name must be between 5 and 20 symbols long.");
            }

            if (repo.RepositoryType != RepositoryPrivateType && repo.RepositoryType!= RepositoryPublicType)
            {
                errors.Add("Repository must be Public or Private.");
            }

            return errors;
        }

    }
}
