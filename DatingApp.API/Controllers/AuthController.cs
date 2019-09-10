using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config= config;
            _repo = repo;

        }

        [HttpPost("Register")]

        public async Task<IActionResult> Register(UserforRegisterDtos userforRegisterDto)
        {
            userforRegisterDto.Username = userforRegisterDto.Username.ToLower();
            if (await _repo.UserExists(userforRegisterDto.Username))
                return BadRequest("User already Exists");

            var usertoCreate = new User
            {
                Username = userforRegisterDto.Username
            };
            var createdUser = await _repo.Register(usertoCreate, userforRegisterDto.Password);
            return StatusCode(201);

        }

        [HttpPost("Login")]

        public async Task<IActionResult> Login(UserforLoginDto userforLoginDto)
        {
            var userFromRepo = await _repo.Login(userforLoginDto.Username, userforLoginDto.Password);

            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
               new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
               new Claim(ClaimTypes.Name,userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor=new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(1),
                SigningCredentials=creds
            };

            var tokenHandler= new JwtSecurityTokenHandler();
            var token=tokenHandler.CreateToken(tokenDescriptor);


            return Ok(new {
                token=tokenHandler.WriteToken(token)
            });


        }
    }
}