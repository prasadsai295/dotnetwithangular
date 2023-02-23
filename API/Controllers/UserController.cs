using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiVersion("1")]
    public class UserController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();
            var userDtoList = _mapper.Map<List<MemberDto>>(users);
            return StatusCode(userDtoList.Any() ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, userDtoList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            var userDto = _mapper.Map<MemberDto>(user);
            return StatusCode(userDto != null ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, userDto);
        }
         [HttpGet(template: "{username}/user")]
        public async Task<IActionResult> GetUserByName(string username)
        {
            var user = await _userRepository.GetMemberAsync(username);
            // var userDto = _mapper.Map<MemberDto>(user);
            return StatusCode(user != null ? StatusCodes.Status200OK : StatusCodes.Status404NotFound, user);
        }

    }
}