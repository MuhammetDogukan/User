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
        public Task<IEnumerable<GetUserDto>> GetUsers();
        public Task<GetUserDto> GetUserById(int id);
        public Task<GetUserDto> CreateUser(CreateUserDto user);
        public Task UpdateUser(UpdateUserDto userUpdate, string id);
        public Task DeleteUser(int id);
        public Task<string> LoginAsync(LoginDto model);
    }
}
