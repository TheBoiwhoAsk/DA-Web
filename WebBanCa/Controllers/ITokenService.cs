using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBanCa.Models;

namespace WebBanCa.Controllers
{
    public interface ITokenService
    {
        string CreateToken(NewUserModel user);
    }
}