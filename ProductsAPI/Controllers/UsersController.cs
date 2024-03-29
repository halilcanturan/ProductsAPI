﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductsAPI.DTO;
using ProductsAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configaration;
        public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configaration = configuration;

        }


        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(UserDTO model)
        {
            var user = new AppUser
            {
                FullName=model.FullName,
                UserName = model.UserName,
                Email = model.Email,                
                DateAdded = DateTime.Now,
            };
            var result = await _userManager.CreateAsync(user,model.Password);

            if (result.Succeeded)
            {
                return StatusCode(201);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var user =await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                return BadRequest(new { message = "email hatalı" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if(result.Succeeded)
            {
                return Ok(
                    new { token = GenerateJWT(user) }
                );
            }
            return Unauthorized();
        }

        private object GenerateJWT(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key =Encoding.ASCII.GetBytes(_configaration.GetSection("AppSettings:Secret").Value ?? ""); //appsetting içindeki secret a gidiyor
            var tokenDescriptior = new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(
                    new Claim[]
                    { 
                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName ?? ""), 
                    }),
                Expires = DateTime.Now.AddDays(1), //süresi bitiş
                SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptior);
            return tokenHandler.WriteToken(token);
        }
    }
}
