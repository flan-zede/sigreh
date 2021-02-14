using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class DepartmentCreate
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int RegionID { get; set; }
    }

    public class DepartmentResponse : DepartmentCreate
    {
        public int Id { get; set; }

        public Region Region { get; set; }

        public ICollection<Subprefecture> Subprefectures { get; set; }
    }

    public class DepartmentUpdate : DepartmentCreate
    {
    }
}
