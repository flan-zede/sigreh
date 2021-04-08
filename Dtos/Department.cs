using sigreh.Models;
using System.Collections.Generic;

namespace sigreh.Dtos
{
    public class DepartmentResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int RegionId { get; set; }

        public Region Region { get; set; }

        public IList<City> Cities { get; set; }

        public IList<User> Users { get; set; }
    }
}
