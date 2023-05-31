
using System.ComponentModel.DataAnnotations;

namespace SocialAPI.Models.Dto
{
    public class StoriaDTORest
    {
     
        public UsernameAndImageDTO User { get; set; } = null!;
        public IEnumerable<StoriaDTO> listaStorie { get; set; } = null!;
    }
}
