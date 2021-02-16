using sigreh.Models;
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
        public DateTime Birthdate { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Idnumber { get; set; }
        public string IdnumberNature { get; set; }
        public string Phone { get; set; }
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
        
        public ICollection<Region> Regions { get; set; }

        public ICollection<Department> Departments { get; set; }

        public ICollection<Subprefecture> Subprefectures { get; set; }

        public ICollection<Establishment> Establishments { get; set; }
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
