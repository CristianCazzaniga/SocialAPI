using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Commento
    {
        public int Id { get; set; }
        public string Contenuto { get; set; } = null!;
        public DateTime DataPubblicazione { get; set; }
        public DateTime DataModifica { get; set; }
        public string fk_user { get; set; } = null!;
        [ForeignKey(nameof(fk_user))]
        public ApplicationUser ApplicationUser { get; set; } = null!;
        [Required]
        public int fk_post { get; set; }
        [ForeignKey(nameof(fk_post))]
        public Post Post { get; set; } = null!;
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
