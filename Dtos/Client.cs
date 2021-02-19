using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class ClientCreate
    {
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
        public int EstablishmentId { get; set; }

        [Required]
        public int UserId { get; set; }
    }

    public class ClientResponse : ClientCreate
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public Establishment Establishment { get; set; }
        
        public User User { get; set; }
        
        public ICollection<Partner> Partners { get; set; }
        
    }

    public class ClientUpdate : ClientCreate
    {
    }

    public class REHClientResponse: ClientResponse
    {
    }

    public class PPClientResponse: ClientResponse
    {
    }

    public class DDMTClientResponse: ClientResponse
    {
    }

    public class DRMTClientResponse: ClientResponse
    {
    }

    public class DSMTClientResponse: ClientResponse
    {
    }

    public class SMIClientResponse: ClientResponse
    {
    }
}
