using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TeisterMask.DataProcessor.ImportDto
{
    public class EmployeeDto
    {
        [Required]
        [StringLength(40, MinimumLength = 3)]
        [JsonProperty("Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [JsonProperty("Email")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^\d{3}-\d{3}-\d{4}$")]
        [JsonProperty("Phone")]
        public string Phone { get; set; }
        [JsonProperty("Tasks")]
        public int[] Tasks { get; set; }
    }
}
