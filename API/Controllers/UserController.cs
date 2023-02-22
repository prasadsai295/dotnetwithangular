using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiVersion("1")]
    public class UserController : BaseApiController
    {
        private readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.Select(x => new AppUser { Id = x.Id, UserName = x.UserName }).ToListAsync();
            return StatusCode(users.Any() ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, users.ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.Where(x => x.Id == id).Select(x => new AppUser { Id = x.Id, UserName = x.UserName }).SingleOrDefaultAsync();
            return StatusCode(user != null ? StatusCodes.Status200OK : StatusCodes.Status404NotFound);
        }

         [HttpGet("not-found")]
         [AllowAnonymous]
        public IActionResult Notfoud(int id)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpGet("server-error")]
        [AllowAnonymous]
        public ActionResult<string> GetServerError()
        {
            var thing = _context.Users.Find(-1);

            var thingToReturn = thing.ToString();

            return thingToReturn;
        }
    }
}