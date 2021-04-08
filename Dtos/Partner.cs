using sigreh.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sigreh.Dtos
{
    public class PartnerCreate
    {
        [Required]
        public string Gender { get; set; }

        public string name { get; set; }

        public int Age { get; set; }

        [Required]
        public int ClientId { get; set; }
    }

    public class PartnerResponse : PartnerCreate
    {
        public int Id { get; set; }

        public IList<Client> Client { get; set; }
    }
}
