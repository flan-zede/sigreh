﻿using System.ComponentModel.DataAnnotations;

namespace sigreh.Wrappers
{
    public class Login
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
