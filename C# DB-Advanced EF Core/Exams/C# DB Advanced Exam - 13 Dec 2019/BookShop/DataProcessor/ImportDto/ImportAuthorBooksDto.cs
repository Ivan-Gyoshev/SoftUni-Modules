using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BookShop.DataProcessor.ImportDto
{
    public class ImportAuthorBooksDto
    {
        [JsonProperty("Id")]
        public int? BookId { get; set; }
    }
}
