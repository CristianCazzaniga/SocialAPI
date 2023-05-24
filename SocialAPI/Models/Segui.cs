using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Segui
    {
        public int Id { get; set; }
        public string Follower { get; set; } = null!;
        public string Seguito { get; set; } = null!;
        [ForeignKey(nameof(Follower))]
        public ApplicationUser UserFollower { get; set; } = null!;
        [ForeignKey(nameof(Seguito))]
        public ApplicationUser UserSeguito { get; set; } = null!;
    }
}
