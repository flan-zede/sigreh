using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sigreh.Models
{
    public class Region
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public IList<Department> Departments { get; set; }

        public IList<User> Users { get; set; }

    }
}
