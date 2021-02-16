using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
        public string Phone { get; set; }

        [Required]
        public DateTime EnterDate { get; set; }

        [Required]
        public int NumberOfNights { get; set; }

        [Required]
        public int NumberOfHours { get; set; }

        [Required]
        public string OccupationType { get; set; }

        [Required]
        public string BedroomNumber { get; set; }

        [Required]
        public string BedroomType { get; set; }

        [Required]
        public int NumberOfVisitors { get; set; }

        public string PartnerGender { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string Signature { get; set; }

        public DateTime CreatedAt { get; set; }

        public int EstablishmentId { get; set; }

        public Establishment Establishment { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

    }
}
