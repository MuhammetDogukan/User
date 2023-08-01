using Application.DtoEntity;
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IUserService
    {
        public Task<IEnumerable<User>> GetUsers();
        public Task<User> GetUserById(int id);
        public Task CreateUser(User user);
        public Task UpdateUser(User user);
        public Task DeleteUser(int id);
        public Task<string> LoginAsync(Login model);
    }
}
