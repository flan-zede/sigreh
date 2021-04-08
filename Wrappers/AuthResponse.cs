using sigreh.Dtos;

namespace sigreh.Wrappers
{
    public class AuthResponse
    {
        public UserResponse User { get; set; }

        public string Jwt { get; set; }
    }

}
