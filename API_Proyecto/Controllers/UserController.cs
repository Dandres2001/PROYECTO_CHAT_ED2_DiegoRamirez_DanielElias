using API_Proyecto.Models;
using API_Proyecto.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return Ok(await db.GetAllUsers());
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByUsername (string username) 
        {
            List<Users> users = db.GetAllUsers().Result.ToList();
            var u = users.Find(x => x.Username == username);
            return Ok(await db.GetUserById(u.id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser (Users user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            await db.AddUser(user);
            return Created("Created", true) ;
        }

        //PUT api/<UserController>/5
        [HttpPut("{username}")]
        public async Task<IActionResult> Put(string username, [FromBody] Users user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            List<Users> users = db.GetAllUsers().Result.ToList();
            var u = users.Find(x => x.Username == username);
            string _id = u.id.ToString();
            user.id = new MongoDB.Bson.ObjectId(_id);
            await db.UpdateUser(user);
            return Created("Created", true);
        }


    }
}
