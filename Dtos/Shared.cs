using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class Login
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class AuthResponse
    {
        public UserResponse User { get; set; }
        
        public string Jwt { get; set; }
    }

    public class Relation
    {
        [Required]
        public string Path { get; set; }
        
        [Required]
        public string Action { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        public int RelatedId { get; set; }
    }
}
