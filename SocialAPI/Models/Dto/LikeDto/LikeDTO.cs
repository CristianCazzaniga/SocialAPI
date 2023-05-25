using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models.Dto
{
    public class LikeDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string TipoDestinazione { get; set; } = null!;  
        public int IdFk { get; set; }
       

    }
}
