using Application.DtoEntity;
using Domain.DtoEntity;
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
        public Task<IEnumerable<GetUser>> GetUsers();
        public Task<GetUser> GetUserById(int id);
        public Task<GetUser> CreateUser(CreateUser user);
        public Task UpdateUser(UpdateUser userUpdate, string id);
        public Task DeleteUser(int id);
        public Task<string> LoginAsync(Login model);
    }
}
