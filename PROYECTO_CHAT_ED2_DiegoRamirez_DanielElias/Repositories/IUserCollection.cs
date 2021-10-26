using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Models;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Repositories
{
    interface IUserCollection
    {

        void createUser(Users user);

        List<Users> GetAllUsers();

    }
}
