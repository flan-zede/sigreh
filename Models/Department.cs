using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int RegionId { get; set; }

        public Region Region { get; set; }

        public IList<City> Cities { get; set; }

        public IList<User> Users { get; set; }
    }
}
