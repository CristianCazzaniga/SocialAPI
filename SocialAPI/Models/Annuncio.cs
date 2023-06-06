using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Annuncio
    {
        public int Id { get; set; }
        public string Titolo { get; set; } = null!;
        public string Descrizione { get; set; } = null!;
        public string Media { get; set; } = null!;

    }
}
