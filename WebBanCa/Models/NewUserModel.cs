    using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebBanCa.Models
{
    public class NewUserModel : IdentityUser
    {
        //public string UserName { get; set; }
        //public string Email { get; set; }
        //public string Token { get; set; }

        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? SDT { get; set; }
        public string? Age { get; set; }

    }
}