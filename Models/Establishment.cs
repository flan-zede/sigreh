using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Models
{
    public class Establishment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Nature { get; set; }

        public string Street { get; set; }

        public string Municipality { get; set; }

        public string Location { get; set; }

        public string Managers { get; set; }
        public string Receptionists { get; set; }

        public int CityID { get; set; }

        public City City { get; set; }
    }
}
