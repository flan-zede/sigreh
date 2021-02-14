using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class RegionCreate
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public int DistrictID { get; set; }
    }

    public class RegionResponse : RegionCreate
    {
        public int Id { get; set; }
        
        public District District { get; set; }
        public ICollection<Department> Departments { get; set; }
    }

    public class RegionUpdate : RegionCreate
    {
    }
}
