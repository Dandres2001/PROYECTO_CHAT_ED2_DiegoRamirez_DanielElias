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
        private IUserCollection dbUsers = new UserCollection();
        private IChatRoomCollection dbChats = new ChatRoomCollection();


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(JsonSerializer.Serialize(await dbUsers.GetAllUsers()));
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByUsername (string username) 
        {
            List<Users> users = dbUsers.GetAllUsers().Result.ToList();
            var u = users.Find(x => x.Username == username);
            return Ok(JsonSerializer.Serialize(await dbUsers.GetUserById(u.id.ToString())));
        }

        [HttpPost]
        public async Task<IActionResult> AddUser (JsonElement jsonUser)
        {

            //Validacion Usuarios
            var userList = new Users();
            string json = jsonUser.ToString();
            userList = JsonSerializer.Deserialize<Users>(json);
            List<Users> users = dbUsers.GetAllUsers().Result.ToList();
            foreach(Users u in users)
            {
                if(u.Username == userList.Username)
                {
                    return BadRequest();
                }
            }

            await dbUsers.AddUser(userList);
            return Created("Created", true) ;
        }

        [HttpPost("access/login")]
        public IActionResult Login([FromBody] JsonElement jsonUser)
        {

            //Validacion Usuarios
            var user = new Users();
            string json = jsonUser.ToString();
            user = JsonSerializer.Deserialize<Users>(json);
            List<Users> users = dbUsers.GetAllUsers().Result.ToList();
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
                List<Users> users = dbUsers.GetAllUsers().Result.ToList();
                var u = users.Find(x => x.Username == username);
                string _id = u.id.ToString();
                user.id = new MongoDB.Bson.ObjectId(_id);
                await dbUsers.UpdateUser(user);
                return Created("Updated", true);
            }
            catch
            {
                return BadRequest();
            }
    
        }

        [HttpGet("allchats")]
        public async Task<IActionResult> GetChatRooms()
        {
            return Ok(JsonSerializer.Serialize(await dbChats.GetAllChatRooms()));
        }

        [HttpGet("chats/{username}")]
        public async Task<IActionResult> GetChatByUsername (string username)
        {
            List<ChatRoom> chats = dbChats.GetAllChatRooms().Result.ToList();
            List<ChatRoom> userChats = new List<ChatRoom>();

            foreach(ChatRoom chat in chats)
            {
                if (chat.chatMembers.Contains(username))
                {
                    userChats.Add(chat);
                }
            }

            
            return Ok(JsonSerializer.Serialize(userChats));
        }

        [HttpPost("chats")]
        public async Task<IActionResult> NewChat(JsonElement jsonChatRoom)
        {

            //Validacion Usuarios
            var newchat = new ChatRoom();
            string json = jsonChatRoom.ToString();
            newchat = JsonSerializer.Deserialize<ChatRoom>(json);
 

            await dbChats.NewChatroom(newchat);
            return Created("Created", true);
        }

        //PUT api/<UserController>/5
        [HttpPut("chats/{id}")]
        public async Task<IActionResult> PutChat(string id, JsonElement jsonChat)
        {
            var currentChat = new ChatRoom();
            string json = jsonChat.ToString();
            currentChat = JsonSerializer.Deserialize<ChatRoom>(json);
            if (currentChat == null)
            {
                return BadRequest();
            }
            try
            {
                List<ChatRoom> chatsList = dbChats.GetAllChatRooms().Result.ToList();
                var u = chatsList.Find(x => x.id == id);
                string _id = u.id.ToString();
                //currentChat.id = new MongoDB.Bson.ObjectId(_id);
                await dbChats.EditChat(currentChat);
                return Created("Updated", true);
            }
            catch
            {
                return Conflict();
            }

        }
    }
}
