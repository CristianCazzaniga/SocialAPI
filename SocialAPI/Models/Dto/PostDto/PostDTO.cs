﻿using System.ComponentModel.DataAnnotations;

namespace SocialAPI.Models.Dto
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Media { get; set; } = null!;
        public string Contenuto { get; set; } = null!;
        public DateTime DataPubblicazione { get; set; }
    }
}
