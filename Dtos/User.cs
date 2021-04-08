using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sigreh.Dtos
{
    public class UserInfo
    {
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

        public Boolean Active { get; set; }

        public string Role { get; set; }

    }

    public class UserCreate : UserInfo
    {
        [Required]
        public string Password { get; set; }
    }

    public class UserResponse : UserInfo
    {
        public int Id { get; set; }

        public IList<Region> Regions { get; set; }

        public IList<Department> Departments { get; set; }

        public IList<Establishment> Establishments { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class UserUpdate : UserCreate
    {
    }

}
