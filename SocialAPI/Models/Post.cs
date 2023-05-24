using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Media { get; set; } = null!;
        public string Contenuto { get; set; } = null!;
        public DateTime DataPubblicazione { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string fk_user { get; set; } = null!;
        [ForeignKey(nameof(fk_user))]
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Commento> Commenti { get; set; } = new List<Commento>();

    }
}
