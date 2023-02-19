using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(){
            return Ok(await _context.Users.ToListAsync());
        }

         [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id){
            return Ok(await _context.Users.FindAsync(id));
        }
    }
}