using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class RegionResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Department> Departments { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
