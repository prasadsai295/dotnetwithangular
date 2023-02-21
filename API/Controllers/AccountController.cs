using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    [ApiVersion("1")]
    public class AccountController: BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }


        [HttpPost("register")] //POST - /api/account/register?username=&password=
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]UserRequestDto userRequest){
            if(await UserExists(userRequest.UserName))
                return BadRequest("Username already exists");
            using var hmac = new HMACSHA512();
            var user = new AppUser{
                UserName = userRequest.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userRequest.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
             var userDto = new UserDto{
                userName = user.UserName,
                Token = _tokenService.CreateToken(user)
           };
            return StatusCode(StatusCodes.Status201Created,userDto);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto){
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDto.UserName.ToLower());
            if(user == null) return Unauthorized("invalid username or password.");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
           for (int i = 0; i < computedHash.Length; i++)
           {
              if(computedHash[i] != user.PasswordHash[i]) return Unauthorized(value: "invalid username or password.");
           }

           var userDto = new UserDto{
                userName = user.UserName,
                Token = _tokenService.CreateToken(user)
           };
           return StatusCode(StatusCodes.Status200OK,userDto);
        }

        private async Task<bool> UserExists(string userName){
            return await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
        }
    }
}