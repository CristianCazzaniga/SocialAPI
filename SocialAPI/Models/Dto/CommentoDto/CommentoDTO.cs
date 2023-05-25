using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models.Dto
{
    public class CommentoDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Contenuto { get; set; } = null!;
        public DateTime DataPubblicazione { get; set; }
        public DateTime DataModifica { get; set; }
    }

}
