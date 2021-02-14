using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Models
{
    public class District
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public Boolean Autonomy { get; set; }

        public ICollection<Region> Regions { get; set; }
    }
}
