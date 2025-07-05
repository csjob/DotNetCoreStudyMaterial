using System.ComponentModel.DataAnnotations;

namespace DotNetCoreWebAPI.Model.Dto
{
    public class UserRegisterDto
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

    }
}
