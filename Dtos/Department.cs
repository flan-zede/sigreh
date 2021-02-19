using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class DepartmentResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int RegionId { get; set; }

        public Region Region { get; set; }

        public ICollection<City> Cities { get; set; }
        
        public ICollection<User> Users { get; set; }
    }
}
