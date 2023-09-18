using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos.User;
using backend.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (dto.Username is null && dto.Password is null && dto.Email is null)
                return BadRequest("All fields must be field");

            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == dto.Username);

            if (user != null)
                return BadRequest("This username already exists");

            var newUser = _mapper.Map<User>(dto);
            
            await _context.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok("User was created successfully");
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == dto.Username);

            if (user is null)
                return NotFound("Bad username");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.HashPassword))
                return Unauthorized("Bad password");

            string token = CreateToken(user);

            return Ok(token);
        }

        [HttpPatch]
        [Route("PatchUser")]
        [Authorize]
        public async Task<IActionResult> PatchUser([FromBody] UserPatchDto dto)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            bool isUpdated = false;

            var username = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == username);

            var otherUser = await _context.Users.FirstOrDefaultAsync(user => user.Username == dto.NewUsername);

            if (BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.HashPassword))
            {
                if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, user.HashPassword))
                    return BadRequest("This is the same password");
                else if (dto.NewPassword != String.Empty)
                {
                    user.HashPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                    isUpdated = true;
                }
            }

            if (dto.NewUsername != user.Username && dto.NewUsername != String.Empty)
            {
                if (otherUser is null)
                {
                    user.Username = dto.NewUsername;
                    isUpdated = true;
                }
                else
                    return BadRequest("This username already exists");
            }

            if (dto.NewEmail != user.Email && dto.NewEmail != String.Empty)
            {
                user.Email = dto.NewEmail;
                isUpdated = true;
            }

            if (isUpdated)
                user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            if (isUpdated)
                return Ok("User data was updated successfully");
            else
                return BadRequest("Nothing has been changed");
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("JWTID", Guid.NewGuid().ToString())
            };

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }
    }
}
