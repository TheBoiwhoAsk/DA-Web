using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebBanCa.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]

        public string FullName { get; set; }
        public string UserName { get; set; }
        public string? Address { get; set; }
        public string SDT { get; set; }
        public string Age { get; set; }
    }
}