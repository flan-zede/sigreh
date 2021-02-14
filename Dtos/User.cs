using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class UserInfo
    {
        [Required]
        public string Username { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public Boolean Blocked { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Firstname { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Idnumber { get; set; }
        public string IdnumberNature { get; set; }
        public string Phone { get; set; }

        [Required]
        public string Districts { get; set; }
        public string Regions { get; set; }
        public string Departments { get; set; }
        public string Subprefectures { get; set; }
        public string Cities { get; set; }
        public string Establishments { get; set; }
    }

    public class UserCreate: UserInfo
    {
        [Required]
        public string Password { get; set; }
    }

    public class UserResponse: UserInfo
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserUpdate : UserCreate
    {
    }

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
}
