using API_Proyecto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Proyecto.Repositories
{
    interface IUserCollection
    {
        Task AddUser(Users user);

        Task UpdateUser(Users user);

        Task<List<Users>> GetAllUsers();

        Task<Users> GetUserById(string id);

    }
}
