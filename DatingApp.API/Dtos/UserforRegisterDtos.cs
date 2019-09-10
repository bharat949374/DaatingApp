using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserforRegisterDtos
    {
        [Required(ErrorMessage="Username should not be empty")]
        public string Username { get; set; }
        [Required(ErrorMessage="Username should not be empty")]
        public string Password { get; set; }
    }
}


