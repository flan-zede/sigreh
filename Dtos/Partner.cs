using sigreh.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sigreh.Dtos
{
    public class PartnerCreate
    {
        [Required]
        public string Gender { get; set; }

        public string name { get; set; }

        public int Age { get; set; }
        
        [Required]
        public int UserId { get; set; }
    }

    public class PartnerResponse : PartnerCreate
    {
        public int Id { get; set; }
    }
}
