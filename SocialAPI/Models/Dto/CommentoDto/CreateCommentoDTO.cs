﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Models.Dto
{
    public class CreateCommentoDTO
    {
        public string Contenuto { get; set; } = null!;
    }

}
