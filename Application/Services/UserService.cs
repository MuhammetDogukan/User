using Application.DtoEntity;
using Application.IServices;
using Application.RabbitMQ;
using Application.TokenService;
using AutoMapper;
using Domain.DtoEntity;
using Domain.Entity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Application.MainHub;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using Application.Enums;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Security.Permissions;
using Microsoft.AspNetCore.Http;
using static System.Net.WebRequestMethods;
using System.Net;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserContext _userContext;
        private readonly IValidator<User> _validator;
        private readonly ITokenService _tokenService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<MessageHub, IMessageHub> _messageHub;
        private readonly SignInManager<User> _signInManager;

        public UserService(IUserContext userContext, IValidator<User> validator, ITokenService tokenService, IRabbitMQService rabbitMQService, 
            IDistributedCache distributedCache, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userContext = userContext;
            _validator = validator;
            _tokenService = tokenService;
            _rabbitMQService = rabbitMQService; 
            _distributedCache = distributedCache;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        

        public async Task<IEnumerable<GetUserDto>> GetUsers()
        {

            var cacheKey = "AllUsers";
            var cachedUsers = await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedUsers))
            {
                // Önbellekten veriyi al ve dön
                var userDtos = _mapper.Map<IEnumerable<GetUserDto>>(JsonConvert.DeserializeObject<IEnumerable<User>>(cachedUsers));
                return userDtos;
            }
            else
            {
                var users = await _userContext.Users.ToListAsync();
                var userDtos = _mapper.Map<IEnumerable<GetUserDto>>(users);

                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) 
                };
                var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userDtos));
                await _distributedCache.SetAsync(cacheKey, byteData, cacheEntryOptions);

                return userDtos;
            }
        }

        public async Task<GetUserDto> GetUserById(int id)
        {

            var cacheKey = $"User_{id}";
            var cachedUserDto = await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedUserDto))
            {
                // Önbellekten veriyi al ve dön
                var userDto = _mapper.Map<GetUserDto>(JsonConvert.DeserializeObject<User>(cachedUserDto));
                return userDto;
            }
            else
            {
                var user = await _userContext.Users.FindAsync(id);
                if (user == null)
                {
                    throw new ArgumentException("User didn't find");
                }

                var userDto = _mapper.Map<GetUserDto>(user);

                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) 
                };
                var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userDto));
                await _distributedCache.SetAsync(cacheKey, byteData, cacheEntryOptions);

                return userDto;
            }
        }
        public async Task<GetUserDto> CreateUser(CreateUserDto createUser)
        {
            string[] userName = createUser.Email.Split("@"); 
            var user = _mapper.Map<User>(createUser);
            user.UserName = userName[0];
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var validationResult = await _validator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            user.SecurityStamp = Guid.NewGuid().ToString();

            if (createUser.Role == RoleEnum.User.ToString())
                await _userManager.AddToRoleAsync(user, RoleEnum.User.ToString());
            else if (createUser.Role == RoleEnum.Admin.ToString())
            {
                await _userManager.AddToRoleAsync(user, RoleEnum.Admin.ToString());
                await _messageHub.Clients.All.AddAdminGroup();
            }

            else if (createUser.Role == RoleEnum.SuperUser.ToString())
                await _userManager.AddToRoleAsync(user, RoleEnum.SuperUser.ToString());
            else
                throw new Exception("Please enter accepteable role");

            //await _messageHub.Clients.Group(RoleEnum.Admin.ToString()).SendMesssageToAdmins($"{user.UserName} has joined");

            /*
            MessageHub messageHub = new MessageHub();
            var a = messageHub.nameAndConnectedId;
            foreach (var b in a)
            {
                    
            }
                
            await _messageHub.Clients.User("").SendMesssageToUser($"{user.UserName} has joined","1");
            */



            //string Password=Guid.NewGuid().ToString("N").Substring(0, 6).ToLower();
            string Password = "123123";


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebUtility.UrlEncode(token);

            var url = $"https://localhost:7161/api/User/{user.Id}/{token}";
            var mailMessage = "Welcome to the system " + user.UserName + ", your password " + user.PasswordHash+", to onfirm your email click here "+url+"."+user.Email;
            _rabbitMQService.SendMessage(mailMessage);

            user.IsDeleted = false;


            var cacheKey = $"User_{user.Id}";
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            };
            var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));
            await _distributedCache.SetAsync(cacheKey, byteData, cacheEntryOptions);



            await _userManager.CreateAsync(user, Password);


            var getUserDto = _mapper.Map<GetUserDto>(user);


            return getUserDto;

        }

        public async Task UpdateUser(UpdateUserDto updateUser, string id)
        {
            if (updateUser == null)
            {
                throw new ArgumentNullException(nameof(updateUser));
            }
            var userQuery = await _userContext.Users.FindAsync(id);
            if (userQuery == null)
            {
                throw new Exception("User didn't find");
            }


            var user = _mapper.Map<User>(updateUser);
            var validationResult = await _validator.ValidateAsync(user);

            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            var cacheKey = $"User_{user.Id}";
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            };
            var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));
            await _distributedCache.SetAsync(cacheKey, byteData, cacheEntryOptions);

            _userContext.Users.Update(user);
            await _userContext.SaveChangesAsync();

            
        }

        public async Task DeleteUser(int id)
        {
            var user = await _userContext.Users.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                _userContext.Users.Update(user);
                await _userContext.SaveChangesAsync();
            }
        }
        public async Task<string> LoginAsync(LoginDto model)
        {
            
            var user = await _userContext.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);
            
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new ArgumentException("User needs to verify account from email");
            }
            if (user == null)
            {
                throw new ArgumentException("Invalid username or password.");
            }

            var passwordResult = await _signInManager.PasswordSignInAsync(user, model.PasswordHash, false, false);

            if (!passwordResult.Succeeded)
            {
                throw new ArgumentException("Invalid username or password.");
            }

            string accessToken = _tokenService.CreateToken(user);


            return accessToken;
        }
        public async Task<string> ActivateAcount(int id, string token)
        {
            token = WebUtility.UrlDecode(token);
            var user = await _userContext.Users.FindAsync(id);
            if (user == null)
            {
                return "<p>User didn't find</p>";
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title>Verification Successful</title>\r\n</head>\r\n<body>\r\n    <h1>Profile Verified</h1>\r\n    <p>Thank you for verifying your profile.</p>\r\n</body>\r\n</html>";
            }
            return "<p>There was a error</p>";
        }
    }
}