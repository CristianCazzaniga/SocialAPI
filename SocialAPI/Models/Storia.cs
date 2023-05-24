using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Storia
    {
        public int Id { get; set; }
        public string Media { get; set; } = null!;
        public DateTime DataPubblicazione { get; set; }
        public string fk_user { get; set; } = null!;
        [ForeignKey(nameof(fk_user))]
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public virtual ICollection<LikeDTO> Likes { get; set; } = new List<LikeDTO>();
    }
}
