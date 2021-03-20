using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace sigreh.Wrappers
{
    public class Relationship
    {
        [Required]
        public string Table { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        public Boolean Add { get; set; }
    }
}
