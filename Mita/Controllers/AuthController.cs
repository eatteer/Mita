using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mita.DTOs;
using Mita.Models;
using Mita.Services;

namespace Mita.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly MitaContext _mitaContext;

        public AuthController(IConfiguration configuration, MitaContext mitaContext)
        {
            _configuration = configuration;
            _mitaContext = mitaContext;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(RegisterUserDTO registerUserDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check if username is already taken
            User? foundUser = await _mitaContext.Users
                .Where(user => registerUserDTO.Username == user.Username)
                .FirstOrDefaultAsync();

            if (foundUser != null) return BadRequest("Username is already taken");

            // Create user and its password hash
            HashingService.CreatePasswordHash(registerUserDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User();
            user.Username = registerUserDTO.Username;
            // http://codeissue.com/issues/i24dd7c2da0e61c/problem-using-encoding-utf8-getstring-and-encoding-utf8-getbytes
            user.PasswordHash = Convert.ToBase64String(passwordHash);
            user.PasswordSalt = Convert.ToBase64String(passwordSalt);
            user.Role = "Guest";

            // Try saving user in database
            await _mitaContext.Users.AddAsync(user);
            int entriesWritten = await _mitaContext.SaveChangesAsync();
            if (entriesWritten == 0) Problem();

            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginUserDTO loginUserDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check if user is registered
            User? user = await _mitaContext.Users
                .Where(user => loginUserDTO.Username == user.Username)
                .FirstOrDefaultAsync();

            if (user == null) return BadRequest("Username or password do not match");

            // Validate password
            // http://codeissue.com/issues/i24dd7c2da0e61c/problem-using-encoding-utf8-getstring-and-encoding-utf8-getbytes
            if (!HashingService.VerifyPasswordHash(loginUserDTO.Password,
                Convert.FromBase64String(user.PasswordHash),
                Convert.FromBase64String(user.PasswordSalt))
            ) return BadRequest("Username or password do not match");

            // Create JWT
            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            JwtSecurityToken token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: credentials);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
