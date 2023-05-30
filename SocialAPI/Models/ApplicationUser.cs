using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = null!;
        public string ImmagineProfilo { get; set; }=null!;
        public DateTime dataDiNascita { get; set; } 
        public virtual ICollection<Storia> Storie { get; set; } = new List<Storia>();
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Commento> Commenti { get; set; } = new List<Commento>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<Messaggio> MessaggiMandati { get; set; } = new List<Messaggio>();
    }
}
