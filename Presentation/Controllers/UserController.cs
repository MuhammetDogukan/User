using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DtoEntity;
using Application.IServices;
using AutoMapper;
using Domain.DtoEntity;
using Domain.Entity;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using Application.MainHub;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        //[Authorize(Roles = "Admin, SupersUser")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }
        //[Authorize(Roles = "Admin, User, SupersUser")]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);  
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser(CreateUserDto createUser)
        {
            var createdUserDto = await _userService.CreateUser(createUser);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUserDto.Id }, createdUserDto);
        }
        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(string id, UpdateUserDto updateUser)
        {
            await _userService.UpdateUser(updateUser, id);
            return NoContent();            
        }
        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUser(id);
            return NoContent();            
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto login)
        {
            string token = await _userService.LoginAsync(login);
            return Ok(token);
        }
        [HttpPut("{id}/{token}")]
        public async Task<ContentResult> ActivateAccount(int id, string token)
        {
            var html = await _userService.ActivateAcount(id, token);
            return new ContentResult
            {
                Content = html,
                ContentType = "text/html"
            };
        }
    }
}
