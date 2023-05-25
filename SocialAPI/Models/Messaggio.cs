using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Messaggio
    {
        public int Id { get; set; }
        public string Contenuto { get; set; } = null!;
        public int fk_chat { get; set; } 
        [ForeignKey(nameof(fk_chat))]
        public Chat Chat { get; set; } = null!;
        public string fk_Mittente { get; set; } = null!;
        [ForeignKey(nameof(fk_Mittente))]
        public ApplicationUser Mittente { get; set; } = null!;
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
