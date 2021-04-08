using sigreh.Models;
using System.Collections.Generic;

namespace sigreh.Dtos
{
    public class RegionResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<Department> Departments { get; set; }

        public IList<User> Users { get; set; }
    }
}
