using Application.DtoEntity;
using Application.IServices;
using Application.RabbitMQ;
using Application.TokenService;
using Domain.DtoEntity;
using Domain.Entity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserContext _userContext;
        private readonly IValidator<User> _validator;
        private readonly ITokenService _tokenService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IConsumer _consumer;
        public UserService(IUserContext userContext, IValidator<User> validator, ITokenService tokenService, IRabbitMQService rabbitMQService, IConsumer consumer)
        {
            _userContext = userContext;
            _validator = validator;
            _tokenService = tokenService;
            _rabbitMQService = rabbitMQService;
            _consumer = consumer;
        }
        

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userContext.Users.ToListAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _userContext.Users.FindAsync(id);
        }

        public async Task CreateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

           
            user.PasswordHash=Guid.NewGuid().ToString("N").Substring(0, 11).ToLower();
            
            var mailMessage = "Welcome to the system " + user.UserName + ", your password " + user.PasswordHash;
            _rabbitMQService.SendMessage(mailMessage);
            await _consumer.GetMessageFromQueue(mailMessage, user.Email);


            //var passwordHasher = new PasswordHasher<User>();
            //user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);
            user.IsDeleted = false;

            _userContext.Users.Add(user);
            await _userContext.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var validationResult = await _validator.ValidateAsync(user);

           

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