using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanCa.Models;
using WebBanCa.Service;
using System.Threading.Tasks;

namespace WebBanCa.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<NewUserModel> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<NewUserModel> _signinManager;

        public AccountController(
            UserManager<NewUserModel> userManager,
            ITokenService tokenService,
            SignInManager<NewUserModel> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signinManager = signInManager;
        }

        // -------------------
        // LOGIN
        // -------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if (user == null)
                return Unauthorized("Invalid username!");

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Username or password is incorrect!");

            return Ok(new
            {
                user.UserName,
                user.Email,
                Token = _tokenService.CreateToken(user)
            });
        }

        // -------------------
        // REGISTER
        // -------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check if username or email already exists
            var userExists = await _userManager.FindByNameAsync(registerDto.Username);
            if (userExists != null)
                return BadRequest("Username already exists");

            var emailExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (emailExists != null)
                return BadRequest("Email already exists");

            var user = new NewUserModel
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return StatusCode(500, result.Errors);

            // Gán role mặc định nếu có (phải đảm bảo role "User" đã tồn tại)
            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new
            {
                user.UserName,
                user.Email,
                Token = _tokenService.CreateToken(user)
            });
        }
    }
}
