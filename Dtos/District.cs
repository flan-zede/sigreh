using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class DistrictCreate
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public Boolean Autonomy { get; set; }
    }

    public class DistrictResponse : DistrictCreate
    {
        public int Id { get; set; }

        public ICollection<Region> Regions { get; set; }
    }

    public class DistrictUpdate : DistrictCreate
    {
    }
}
