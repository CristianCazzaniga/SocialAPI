using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models.Dto
{
    public class SeguiDTO
    {
        public string FollowerNome { get; set; } = null!;
        public string SeguitoNome { get; set; } = null!;
    }
}
