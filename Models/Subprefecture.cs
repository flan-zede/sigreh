using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Models
{
    public class Subprefecture
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public ICollection<City> Cities { get; set; }
        
        public ICollection<User> Users { get; set; }
    }
}
