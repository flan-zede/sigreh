using System.ComponentModel.DataAnnotations;

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
