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
using Presentation.MainHub;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IDistributedCache _distributedCache;
        private readonly IHubContext<MessageHub, IMessageHub> _messageHub;

        public UserController(IUserService userService, IMapper mapper, UserManager<User> userManager, IDistributedCache distributedCache, IHubContext<MessageHub, IMessageHub> messageHub)
        {
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _distributedCache = distributedCache;
            _messageHub = messageHub;
        }
        [Authorize(Roles = "Admin, SupersUser")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var cacheKey = "AllUsers";
            var cachedUsers = await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedUsers))
            {
                // Önbellekten veriyi al ve dön
                var userDtos = _mapper.Map<IEnumerable<GetUser>>(JsonConvert.DeserializeObject<IEnumerable<User>>(cachedUsers));
                return Ok(userDtos);
            }
            else
            {
                var users = await _userService.GetUsers();
                var userDtos = _mapper.Map<IEnumerable<GetUser>>(users);

                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Örnek olarak 1 saat süreyle önbellekte tutabilirsiniz
                };
                var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userDtos));
                await _distributedCache.SetAsync(cacheKey, byteData, cacheEntryOptions);

                return Ok(userDtos);
            }
        }
        [Authorize(Roles = "Admin, User, SupersUser")]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUser>> GetUserById(int id)
        {
            var cacheKey = $"User_{id}";
            var cachedUserDto = await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedUserDto))
            {
                // Önbellekten veriyi al ve dön
                var userDto = _mapper.Map<GetUser>(JsonConvert.DeserializeObject<User>(cachedUserDto));
                return Ok(userDto);
            }
            else
            {
                var user = await _userService.GetUserById(id);
                if (user == null)
                {
                    return NotFound();
                }

                var userDto = _mapper.Map<GetUser>(user);

                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) // Örnek olarak 1 saat süreyle önbellekte tutabilirsiniz
                };
                var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userDto));
                await _distributedCache.SetAsync(cacheKey, byteData, cacheEntryOptions);

                return Ok(userDto);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateUser(CreateUser createUser)
        {
            try
            {

                var user = _mapper.Map<User>(createUser);



                user.SecurityStamp = Guid.NewGuid().ToString();
                
                if (createUser.Role == "User")
                    await _userManager.AddToRoleAsync(user, "User");
                else if (createUser.Role == "Admin")
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    await _messageHub.Clients.All.AddAdminGroup();
                }
                    
                else if (createUser.Role == "SuperUser")
                    await _userManager.AddToRoleAsync(user, "SuperUser");
                else
                    return BadRequest("Please enter accepteable role");
                
                await _messageHub.Clients.Group("Admin").SendMesssageToAdmins($"{user.UserName} has joined");
                
                //belli bir kullanıcıya mesaj göndermek için
                /*
                MessageHub messageHub = new MessageHub();
                var a = messageHub.nameAndConnectedId;
                string specificUserName ="";
                foreach (var b in a)
                {
                    if ( b=="string" )
                    {
                        specificUserName = a[a.IndexOf(b) + 1];
                    }
                }
                
                await _messageHub.Clients.User(specificUserName).SendMesssageToUser($"{user.UserName} has joined","1");
                */


                await _userService.CreateUser(user);
                var createdUserDto = _mapper.Map<GetUser>(user);


                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, createdUserDto);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the user.");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UpdateUser updateUser)
        {

            try
            {
                var user = _mapper.Map<User>(updateUser);
                await _userService.UpdateUser(user);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the user.");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUser(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(Login login)
        {
            string token = await _userService.LoginAsync(login);
            return Ok(token);
        }
    }
}
