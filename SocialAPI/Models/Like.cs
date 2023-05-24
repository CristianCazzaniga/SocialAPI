using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Like
    {
        public int Id { get; set; }
        public string TipoDestinazione { get; set; } = null!;
        public string fk_user { get; set; } = null!;
        [ForeignKey(nameof(fk_user))]
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public int? fk_post { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(fk_post))]
        public Post Post { get; set; } = null!;
        public int? fk_storia { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(fk_storia))]
        public Storia Storia { get; set; } = null!;
        public int? fk_messaggio { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(fk_messaggio))]
        public Messaggio Messaggio { get; set; } = null!;
        public int? fk_commento { get; set; }
        [ValidateNever]
        [ForeignKey(nameof(fk_commento))]
        public Commento Commento { get; set; } = null!;

    }
}
