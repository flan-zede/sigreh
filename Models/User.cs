using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sigreh.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Firstname { get; set; }

        public DateTime Birthdate { get; set; }

        public string Gender { get; set; }

        public string Nationality { get; set; }

        public string Idnumber { get; set; }

        public string IdnumberNature { get; set; }

        [Required]
        public string PhoneType { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Username { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(5)]
        public string Password { get; set; }

        public Boolean Active { get; set; }

        public string Role { get; set; }

        public IList<Region> Regions { get; set; }

        public IList<Department> Departments { get; set; }

        public IList<Establishment> Establishments { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

    }

}
