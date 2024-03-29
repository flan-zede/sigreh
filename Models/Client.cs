﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sigreh.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public DateTime Birthdate { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Nationality { get; set; }

        [Required]
        public string Idnumber { get; set; }

        [Required]
        public string IdnumberNature { get; set; }

        [Required]
        public string PhoneType { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string OccupationType { get; set; }

        [Required]
        public int NumberOfNights { get; set; }

        [Required]
        public int NumberOfHours { get; set; }

        [Required]
        public string BedroomNumber { get; set; }

        [Required]
        public string BedroomType { get; set; }

        [Required]
        public DateTime EnterDate { get; set; }

        [Required]
        public string EnterTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public IList<Partner> Partners { get; set; }

        public int EstablishmentId { get; set; }

        public Establishment Establishment { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

    }
}
