using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class Annuncio
    {
        public int Id { get; set; }
        public string Titolo { get; set; } = null!;
        public string Descrizione { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Href { get; set; } = null!;
        public double Prezzo { get; set; }
        public DateTime DataPubblicazione { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
