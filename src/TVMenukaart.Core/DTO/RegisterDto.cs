using System.ComponentModel.DataAnnotations;

namespace TVMenukaart.DTO
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
