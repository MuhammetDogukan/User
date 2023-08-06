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

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserContext _userContext;
        private readonly IValidator<User> _validator;
        private readonly ITokenService _tokenService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IConsumer _consumer;
        private readonly IDistributedCache _distributedCache;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<MessageHub, IMessageHub> _messageHub;

        public UserService(IUserContext userContext, IValidator<User> validator, ITokenService tokenService, IRabbitMQService rabbitMQService, 
            IConsumer consumer, IDistributedCache distributedCache, IMapper mapper, UserManager<User> userManager)
        {
            _userContext = userContext;
            _validator = validator;
            _tokenService = tokenService;
            _rabbitMQService = rabbitMQService;
            _consumer = consumer;
            _distributedCache = distributedCache;
            _mapper = mapper;
            _userManager = userManager;
        }
        

        public async Task<IEnumerable<GetUser>> GetUsers()
        {

            var cacheKey = "AllUsers";
            var cachedUsers = await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedUsers))
            {
                // Önbellekten veriyi al ve dön
                var userDtos = _mapper.Map<IEnumerable<GetUser>>(JsonConvert.DeserializeObject<IEnumerable<User>>(cachedUsers));
                return userDtos;
            }
            else
            {
                var users = await _userContext.Users.ToListAsync();
                var userDtos = _mapper.Map<IEnumerable<GetUser>>(users);

                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) 
                };
                var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userDtos));
                await _distributedCache.SetAsync(cacheKey, byteData, cacheEntryOptions);

                return userDtos;
            }
        }

        public async Task<GetUser> GetUserById(int id)
        {

            var cacheKey = $"User_{id}";
            var cachedUserDto = await _distributedCache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedUserDto))
            {
                // Önbellekten veriyi al ve dön
                var userDto = _mapper.Map<GetUser>(JsonConvert.DeserializeObject<User>(cachedUserDto));
                return userDto;
            }
            else
            {
                var user = await _userContext.Users.FindAsync(id);
                if (user == null)
                {
                    throw new ArgumentException("User didn't find");
                }

                var userDto = _mapper.Map<GetUser>(user);

                var cacheEntryOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) 
                };
                var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userDto));
                await _distributedCache.SetAsync(cacheKey, byteData, cacheEntryOptions);

                return userDto;
            }
        }
        public async Task<GetUser> CreateUser(CreateUser createUser)
        {
            var user = _mapper.Map<User>(createUser);

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
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

            await _messageHub.Clients.Group(RoleEnum.Admin.ToString()).SendMesssageToAdmins($"{user.UserName} has joined");

            /*
            MessageHub messageHub = new MessageHub();
            var a = messageHub.nameAndConnectedId;
            foreach (var b in a)
            {
                    
            }
                
            await _messageHub.Clients.User("").SendMesssageToUser($"{user.UserName} has joined","1");
            */

            

           
            user.PasswordHash=Guid.NewGuid().ToString("N").Substring(0, 11).ToLower();
            
            var mailMessage = "Welcome to the system " + user.UserName + ", your password " + user.PasswordHash;
            _rabbitMQService.SendMessage(mailMessage);
            //await _consumer.GetMessageFromQueue(mailMessage, user.Email);


            //var passwordHasher = new PasswordHasher<User>();
            //user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);
            user.IsDeleted = false;


            var cacheKey = $"User_{user.Id}";
            var cacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
            };
            var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));
            await _distributedCache.SetAsync(cacheKey, byteData, cacheEntryOptions);



            _userContext.Users.Add(user);
            await _userContext.SaveChangesAsync();


            var getUserDto = _mapper.Map<GetUser>(user);


            return getUserDto;

        }

        public async Task UpdateUser(UpdateUser updateUser, string id)
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
                _userContext.Users.Remove(user);
                await _userContext.SaveChangesAsync();
            }
        }
        public async Task<string> LoginAsync(Login model)
        {
            
            var user = await _userContext.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);

            if (user == null)
            {
                throw new ArgumentException("Invalid username or password.");
            }

            //var passwordHasher = new PasswordHasher<User>();
            //var passworVerifacition = passwordHasher.VerifyHashedPassword(user, user.PasswordHash,model.PasswordHash);

            if (user.PasswordHash != model.PasswordHash)
            {
                throw new ArgumentException("Invalid username or password.");
            }

            string accessToken = _tokenService.CreateToken(user);


            return accessToken;
        }
    }
}