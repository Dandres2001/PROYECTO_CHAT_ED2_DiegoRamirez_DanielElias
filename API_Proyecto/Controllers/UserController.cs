using API_Proyecto.Models;
using API_Proyecto.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Text;
using System.Text.Json;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Proyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //METODOS PARA MANEJO DE USUARIOS CON MONGO
        private IUserCollection db = new UserCollection(); 
        
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(JsonSerializer.Serialize(await db.GetAllUsers()));
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByUsername (string username) 
        {
            List<Users> users = db.GetAllUsers().Result.ToList();
            var u = users.Find(x => x.Username == username);
            return Ok(JsonSerializer.Serialize(await db.GetUserById(u.id.ToString())));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser (JsonElement jsonUser)
        {

            //Validacion Usuarios
            var userList = new Users();
            string json = jsonUser.ToString();
            userList = JsonSerializer.Deserialize<Users>(json);
            List<Users> users = db.GetAllUsers().Result.ToList();
            foreach(Users u in users)
            {
                if(u.Username == userList.Username)
                {
                    return BadRequest();
                }
            }

            await db.AddUser(userList);
            return Created("Created", true) ;
        }


        
        [HttpPost("access/login")]
        public IActionResult Login([FromBody] JsonElement jsonUser)
        {

            //Validacion Usuarios
            var user = new Users();
            string json = jsonUser.ToString();
            user = JsonSerializer.Deserialize<Users>(json);
            List<Users> users = db.GetAllUsers().Result.ToList();
            foreach (Users u in users)
            {
                if (u.Username == user.Username && u.Password == user.Password)
                {
                    return Ok();
                }
            }


            return BadRequest();
        }

        //PUT api/<UserController>/5
        [HttpPut("{username}")]
        public async Task<IActionResult> Put(string username,JsonElement jsonUser)
        {
            var user = new Users();
            string json = jsonUser.ToString();
            user = JsonSerializer.Deserialize<Users>(json);
            if (user == null)
            {
                return BadRequest();
            }
            try
            {
                List<Users> users = db.GetAllUsers().Result.ToList();
                var u = users.Find(x => x.Username == username);
                string _id = u.id.ToString();
                user.id = new MongoDB.Bson.ObjectId(_id);
                await db.UpdateUser(user);
                return Created("Updated", true);
            }
            catch
            {
                return BadRequest();
            }
    
        }


    }
}
