﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.Entities.ViewModels
{
    public class ResetPasswordVM
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Token { get; set; }

        [Required]
        public string? Password { get; set; }

        [NotMapped]
        [Compare("Password")]
        [Required]
        public string? ConfirmPassword { get; set; }
    }
}
