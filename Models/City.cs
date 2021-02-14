using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int SubprefectureID { get; set; }

        public Boolean DistrictCapital { get; set; }

        public Boolean RegionCapital { get; set; }

        public Boolean DepartmentCapital { get; set; }

        public Boolean SubprefectureCapital { get; set; }

        public Subprefecture Subprefecture { get; set; }
    }
}
