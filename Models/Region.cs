using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Models
{
    public class Region
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public int DistrictID { get; set; }

        public District District { get; set; }
        public ICollection<Department> Departments { get; set; }

    }
}
