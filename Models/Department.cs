using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        public int RegionID { get; set; }

        public Region Region { get; set; }

        public ICollection<Subprefecture> Subprefectures { get; set; }
    }
}
