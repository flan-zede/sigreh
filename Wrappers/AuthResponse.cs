using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using sigreh.Dtos;

namespace sigreh.Wrappers
{
    public class AuthResponse
    {
        public UserResponse User { get; set; }
        
        public string Jwt { get; set; }
    }

}
