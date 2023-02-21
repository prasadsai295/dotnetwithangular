using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
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
        public async Task<IActionResult> GetUsers(){
            return Ok(await _context.Users.Select(x => new {Id = x.Id, UserName = x.UserName }).ToListAsync());
        }

         [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id){
            return Ok(await _context.Users.Where(x => x.Id == id).Select(x => new {Id = x.Id, UserName = x.UserName }).SingleOrDefaultAsync());
        }
    }
}