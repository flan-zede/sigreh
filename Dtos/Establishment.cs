using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class EstablishmentCreate
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Nature { get; set; }

        public string Municipality { get; set; }
        
        public string Street { get; set; }

        public string Location { get; set; }

        public int CityId { get; set; }
    }

    public class EstablishmentResponse : EstablishmentCreate
    {
        public int Id { get; set; }

        public City City { get; set; }
        
        public IList<User> Users { get; set; }
    }

    public class EstablishmentUpdate : EstablishmentCreate
    {
    }
}
