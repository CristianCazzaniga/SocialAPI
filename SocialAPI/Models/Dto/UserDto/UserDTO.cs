using System.Globalization;

namespace SocialAPI.Models.Dto
{
    public class UserDTO
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public DateTime dataDiNascita { get; set; }
        public string ImmagineProfilo { get; set; }
        public string Name { get; set; }
    }
}
