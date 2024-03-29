﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sigreh.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public IList<Establishment> Establishments { get; set; }
    }
}
