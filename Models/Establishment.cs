using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public string Municipality { get; set; }

        public string Location { get; set; }

        public int CityId { get; set; }

        public City City { get; set; }

        public IList<User> Users { get; set; }

    }
}
