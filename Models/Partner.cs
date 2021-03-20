using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Models
{
    public class Partner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Gender { get; set; }

        public string name { get; set; }

        public int Age { get; set; }
        
        [Required]
        public int ClientId { get; set; }

        public Client Client { get; set; }
    }
}
