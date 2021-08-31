namespace BattleCards.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using static Data.DataConstants;

    public class Card
    {
        [Key]
        [Required]
        [MaxLength(IdMaxLength)]
        public int Id { get; set; }

        [Required]
        [MaxLength(CardMaxName)]
        public string Name { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public string Keyword { get; set; }

        [Required]
        public int Attack { get; set; }

        [Required]
        public int Health { get; set; }

        [Required]
        [MaxLength(CardDescriptionMax)]
        public string Description { get; set; }

        public ICollection<UserCard> UserCard { get; set; } = new List<UserCard>();
    }
}
