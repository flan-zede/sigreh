using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class SubprefectureCreate
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int DepartmentId { get; set; }
    }

    public class SubprefectureResponse : SubprefectureCreate
    {
        public int Id { get; set; }
        
        public Department Department { get; set; }
        
        public ICollection<City> Cities { get; set; }
        
        public ICollection<User> Users { get; set; }
    }

    public class SubprefectureUpdate : SubprefectureCreate
    {
    }
}
