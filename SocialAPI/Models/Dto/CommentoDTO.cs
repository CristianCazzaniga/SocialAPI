using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models
{
    public class CommentoDTO
    {
        public string Contenuto { get; set; } = null!;
    }

}
