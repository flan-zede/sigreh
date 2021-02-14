using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(5)]
        public string Password { get; set; }

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
        public DateTime CreatedAt { get; set; }

        [Required]
        public string Districts { get; set; }
        public string Regions { get; set; }
        public string Departments { get; set; }
        public string Subprefectures { get; set; }
        public string Cities { get; set; }
        public string Establishments { get; set; }
    }
}
