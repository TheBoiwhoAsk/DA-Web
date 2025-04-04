using System.ComponentModel.DataAnnotations;

namespace WebBanCa.Models
{
    public class RegisterModel
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }

        //[Required]
        //public string FullName { get; set; }
        //[Required]
        //public string UserName { get; set; }

        //[Required]
        //[EmailAddress]
        //public string Email { get; set; }

        //[Required]
        //[MinLength(6)]
        //public string Password { get; set; }

    }
}
