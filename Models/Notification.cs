using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sigreh.Models
{  
    public class Notification  
    {  
        [Key]  
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public Boolean Viewed { get; set; }

        public DateTime CreatedAt { get; set; }
    }  
}  