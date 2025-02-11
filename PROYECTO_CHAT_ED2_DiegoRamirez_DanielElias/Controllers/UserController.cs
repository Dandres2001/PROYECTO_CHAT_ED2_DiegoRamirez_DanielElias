﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Models;
using PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Repositories;
using PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Helper;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using LibreriaRD;
using System.IO;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Controllers
{
    public class UserController : Controller
    {

        private IUserCollection db = new UserCollection();
        ApiProyecto _api = new ApiProyecto();
        // GET: UserController
        public async Task<IActionResult> Index()
        {
            List<Users> users = new List<Users>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user");
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                users = System.Text.Json.JsonSerializer.Deserialize<List<Users>>(results);

            }
        
            return View(users);
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            var cifrdadoSDES = new SDES();
            try
            {
                Random rand = new Random();
                var user = new Users();
                {
                    
                    user.Username = collection["Username"];
                    user.Password = collection["Password"];
                    user.key = rand.Next(1, 500).ToString();
                    user.Password = cifrdadoSDES.Cypher(user.key,user.Password);
                    user.eMail = collection["eMail"];
                    user.friendsList = new List<string>();
                    user.requestsList = new List<string>();
                    user.ChatRoomsIds = new Dictionary<string, string>();
                };

                HttpClient client = _api.Initial();
                string json = System.Text.Json.JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

          
                HttpResponseMessage response = await client.PostAsync("api/user", content);
                if (response.IsSuccessStatusCode)
                {

                    ShowDialog("Registro exitoso");
                }
                else
                {
                    ShowDialog("El usuario que desea ya se encuentra en uso, pruebe con otro");
                    return View();
                }





                return RedirectToAction(nameof(Login));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(IFormCollection collection)
        {
            try
            {
                List<Users> usersList = db.GetAllUsers().ToList();
                var user = new Users();
                {
                    //descifrar o cifrar 
                    user.Username = collection["Username"];
                    user.Password = collection["Password"];
                    user.eMail = collection["eMail"];

                };
                HttpClient client = _api.Initial();
                string json = System.Text.Json.JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("api/user/access/login", content);
              
                    if (response.IsSuccessStatusCode)
                    {
                       
                        //ShowDialog("Sesión iniciada como: " + u.Username);
                        HttpContext.Session.SetString("usuarioLogeado", user.Username);
                        ViewBag.sessionv= HttpContext.Session.GetString("usuarioLogeado");
                        //RETORNAR ACCION DE SESION INICIADA
                        return RedirectToAction(nameof(Chat));
                    }
                
                //SI NO INICIA SESIÓN
                ShowDialog("Usuario y/o contraseña incorrectos");

                return View();
             }
            catch
            {
                return View();
    }
}
      

        //ACCIONES PARA AGREGAR CONTACTOS
        [HttpGet]
        public async Task<IActionResult> AddFriend(string searched)
        {

            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            List<Users> users = new List<Users>();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user");
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                users = System.Text.Json.JsonSerializer.Deserialize<List<Users>>(results);

            }
            
            ViewData["GetUser"] = searched;
            var userRequest = from x in users select x;
            if (!String.IsNullOrEmpty(searched))
            {

                //DELEGATES
                userRequest = userRequest.Where(x => x.Username.Contains(searched));

            }
            return View(userRequest);
        }
        #region 
        public async Task<IActionResult> SendRequest(string user)
        {
            var receiver = new Users();
            var sender = new Users();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/"+user);
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                receiver = System.Text.Json.JsonSerializer.Deserialize<Users>(results);

            }
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            res = await client.GetAsync("api/user/" + ViewBag.sessionv);
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                sender = System.Text.Json.JsonSerializer.Deserialize<Users>(results);

            }
            foreach (string u in receiver.requestsList)
            {
                if (u == sender.Username)
                {
                    ShowDialog("Ya has enviado una solicitud a esta persona");
                    return RedirectToAction(nameof(Chat));


                }
            }
            foreach (string u in receiver.friendsList)
            {
                if (u == sender.Username)
                {
                    ShowDialog("Esta persona ya está entre tus contactos");
                    return RedirectToAction(nameof(Chat));


                }
            }

            receiver.requestsList.Add(sender.Username);
            string json = System.Text.Json.JsonSerializer.Serialize(receiver);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync("api/user/"+receiver.Username, content);
            if (response.IsSuccessStatusCode)
            {
                ShowDialog("Solicitud enviada");

            }


            return RedirectToAction(nameof(Chat));
        }
        [HttpGet]
        public async Task<IActionResult> ViewRequests()
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");

            var user = new Users();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/" + ViewBag.sessionv);
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                user = System.Text.Json.JsonSerializer.Deserialize<Users>(results);

            }

            return View(user.requestsList);
        }
        public async Task<IActionResult> AcceptRequest(string user)
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            var requester = new Users();
            var llaves = new Diffie_Hellman();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/" + user);
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                requester = System.Text.Json.JsonSerializer.Deserialize<Users>(results);

            }
            var currentUser = new Users();
            res = await client.GetAsync("api/user/" + ViewBag.sessionv);
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                currentUser = System.Text.Json.JsonSerializer.Deserialize<Users>(results);
            }
            //ELIMINAR AL REQUESTER DE LA LISTA DE REQUESTS
            currentUser.requestsList.Remove(currentUser.requestsList.Find(x => x == requester.Username));
            //AGREGAR AL REQUESTER A LISTA DE AMIGOS DE CURRENT
            currentUser.friendsList.Add(requester.Username);
            //AGREGAR AL CURRRENT A LA LISTA DE AMIGOS DEL CURRENT
            requester.friendsList.Add(currentUser.Username);
            //LLAMAR A UPDATE DESDE LA API PARA CURRENT

   
            string json = System.Text.Json.JsonSerializer.Serialize(currentUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync("api/user/" + currentUser.Username, content);
            //LLAMAR A UPDATE DESDE LA API PARA REQUESTER
            json = System.Text.Json.JsonSerializer.Serialize(requester);
            content = new StringContent(json, Encoding.UTF8, "application/json");
            response = await client.PutAsync("api/user/" + requester.Username, content);
            if (response.IsSuccessStatusCode)
            {
                //CREAR CHATROOM ENTRE LOS DOS CONTACTOS
                var newChatRoom = new ChatRoom();
                newChatRoom.chatMembers = new List<string>();
                
                newChatRoom.messagesList = new List<Messages>();
                var guid = Guid.NewGuid();
                newChatRoom.id = guid.ToString();
                newChatRoom.chatMembers.Add(requester.Username);
                newChatRoom.chatMembers.Add(currentUser.Username);
                newChatRoom.keys = llaves.getpublickey();
                //AGREGAR EL CHAT ROOM EN LA LISTA DE ROOMS DE CADA USUARIO
                currentUser.ChatRoomsIds.Add(newChatRoom.id.ToString(), requester.Username);
                requester.ChatRoomsIds.Add(newChatRoom.id.ToString(), currentUser.Username);
                //ACTUALIZAR USUARIOS Y CHATROOMS EN DB
                //actualizar current
                client = _api.Initial();
                json = System.Text.Json.JsonSerializer.Serialize(currentUser);
                content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PutAsync("api/user/" + currentUser.Username, content);
                //actualizar requester
                json = System.Text.Json.JsonSerializer.Serialize(requester);
                content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PutAsync("api/user/" + requester.Username, content);
                //crear chatroom en db
                json = System.Text.Json.JsonSerializer.Serialize(newChatRoom);
                content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync("api/user/chats", content);

                // ShowDialog("Contacto Agregado");
            }
            return RedirectToAction(nameof(ViewRequests));
        }
        public async Task<IActionResult> DeclineRequest(string user)
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            var requester = new Users();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/" + user);
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                requester = System.Text.Json.JsonSerializer.Deserialize<Users>(results);

            }
            var currentUser = new Users();
            res = await client.GetAsync("api/user/" + ViewBag.sessionv);
            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                currentUser = System.Text.Json.JsonSerializer.Deserialize<Users>(results);
            }
            //ELIMINAR AL REQUESTER DE LA LISTA DE REQUESTS
            currentUser.requestsList.Remove(currentUser.requestsList.Find(x => x == requester.Username));
            //LLAMAR A UPDATE DESDE LA API PARA CURRENT
            string json = System.Text.Json.JsonSerializer.Serialize(currentUser);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync("api/user/" + currentUser.Username, content);

            return RedirectToAction(nameof(ViewRequests));
        }
        #endregion

        // GET: UserController/Create
        public ActionResult CreateGroup()
        {

            return View();
          
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGroup(IFormCollection collection)
        {
            try
            {
                ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
                var currentUser = new Users();
                var RSALlaves = new RSA();
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("api/user/" + ViewBag.sessionv);
                if (res.IsSuccessStatusCode)
                {
                    var results = res.Content.ReadAsStringAsync().Result;
                    currentUser = System.Text.Json.JsonSerializer.Deserialize<Users>(results);
                }
                var newChatRoom = new ChatRoom();
                newChatRoom.GroupName = collection["GroupName"];
                newChatRoom.id = Guid.NewGuid().ToString();
                newChatRoom.chatMembers = new List<string>();
                newChatRoom.messagesList = new List<Messages>();
                newChatRoom.chatMembers.Add(ViewBag.sessionv);
                newChatRoom.keys = (RSALlaves.GenerarLlaves());
                //registrando chatroom en user
                currentUser.ChatRoomsIds.Add(newChatRoom.id, newChatRoom.GroupName);
                //llamando a api
                //actualizar current
                client = _api.Initial();
                var json = System.Text.Json.JsonSerializer.Serialize(currentUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync("api/user/" + currentUser.Username, content);
                //crear chat en db
                json = System.Text.Json.JsonSerializer.Serialize(newChatRoom);
                content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PostAsync("api/user/chats", content);

                return RedirectToAction(nameof(AddToGroup), new { groupId = newChatRoom.id });
            }
            catch
            {
                return View();
            }
        }

        public async Task<IActionResult> AddToGroup(string groupId)
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            var currentUser = new Users();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/" + ViewBag.sessionv);
            var results = res.Content.ReadAsStringAsync().Result;
            currentUser = System.Text.Json.JsonSerializer.Deserialize<Users>(results);

            res = await client.GetAsync("api/user");
            var result = res.Content.ReadAsStringAsync().Result;
            var allUsers = System.Text.Json.JsonSerializer.Deserialize<List<Users>>(result);
            var contactsList = new List<Users>();

            res = await client.GetAsync("api/user/allchats");
            result = res.Content.ReadAsStringAsync().Result;
            var allChats = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result);

            var currentGroup = new ChatRoom();
            foreach(ChatRoom chat in allChats)
            {
                if(chat.id == groupId)
                {
                    currentGroup = chat;
                }
            }
            foreach (Users user in allUsers)
            {
                foreach(string uname in currentUser.friendsList)
                {
                    if(user.Username == uname && !currentGroup.chatMembers.Contains(user.Username))
                    {
                        contactsList.Add(user);
                    }
                }
            }
            ViewData["GroupId"] = groupId;

            return View(contactsList);

        }

     
        public async Task<IActionResult> AddToGroupMethod(string GroupId, string added)
        {
            try
            {
                ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");

                
                HttpClient client = _api.Initial();
                HttpResponseMessage res = await client.GetAsync("api/user/" + added);
                var results = res.Content.ReadAsStringAsync().Result;
                var AddedUser = System.Text.Json.JsonSerializer.Deserialize<Users>(results);

                res = await client.GetAsync("api/user/allchats");
                var result = res.Content.ReadAsStringAsync().Result;
                var allChatRooms = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result);
                var chatRoom = new ChatRoom();

                foreach (ChatRoom chat in allChatRooms)
                {
                    if (chat.id.ToString() == GroupId)
                    {
                        chatRoom = chat;
                    }
                }

                chatRoom.chatMembers.Add(added);

                AddedUser.ChatRoomsIds.Add(GroupId, chatRoom.GroupName);

                //ACTUALIZAR USER AGREGADO A GRUPO

                var json = System.Text.Json.JsonSerializer.Serialize(AddedUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync("api/user/" + AddedUser.Username, content);
                //ACTUALIZAR CHAT ROOM
                json = System.Text.Json.JsonSerializer.Serialize(chatRoom);
                content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await client.PutAsync("api/user/chats/" + chatRoom.id, content);

                return RedirectToAction(nameof(AddToGroup), new { groupId = GroupId});
            }
            catch
            {
                return View();
            }
        }


        public async Task<IActionResult> Chat()
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            var currentUser = new Users();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/" + ViewBag.sessionv);
            if (res.IsSuccessStatusCode)
            {
                var result = res.Content.ReadAsStringAsync().Result;
                currentUser= System.Text.Json.JsonSerializer.Deserialize<Users>(result);

            }

            //OBTENER LOS NOMBRES DE LOS CHATS A MOSTRAR
            res = await client.GetAsync("api/user/allchats");
            var result1 = res.Content.ReadAsStringAsync().Result;
            var AllChatRooms = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result1);


       
            

            return View(currentUser.ChatRoomsIds.Values.ToList());
        }


        public async Task<IActionResult> Room(string id)
        {

            var decypherSDES = new SDES();
            var decypherRSA = new RSA();

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/allchats");
            var result = res.Content.ReadAsStringAsync().Result;
            var allChatRooms = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result);
            var chatRoom = new ChatRoom();
            List<string> mensajesdescifrados = new List<string>();
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            var currentUser = new Users();
            var keys = new Diffie_Hellman();
            client = _api.Initial();
            res = await client.GetAsync("api/user/" + ViewBag.sessionv);
            if (res.IsSuccessStatusCode)
            {
                result = res.Content.ReadAsStringAsync().Result;
                currentUser = System.Text.Json.JsonSerializer.Deserialize<Users>(result);

            }
            string roomId = currentUser.ChatRoomsIds.FirstOrDefault(x => x.Value == id).Key;
            ViewData["ChatWith"] = id;
            foreach (ChatRoom chat in allChatRooms)
            {
                if (chat.id.ToString() == roomId)
                {
                   //aqui
                    chatRoom = chat;
                    //aqui se descifran los mensajes
                    if (chatRoom.GroupName == null)
                    {
                        foreach (Messages s in chatRoom.messagesList)
                        {
                            if (s.isFile == false)
                            {

                                string descifrado = decypherSDES.Decypher(keys.getprivatekey(chatRoom.keys), s.Text);
                                s.Text = descifrado;

                            }


                        }
                    }
                    else
                    {
                        foreach (Messages s in chatRoom.messagesList)
                        {

                            if (s.isFile== false)
                            {
                                string descifrado = decypherRSA.RSA_DECYPHER(s.Text, Convert.ToInt32(chatRoom.keys[0]), Convert.ToInt32(chatRoom.keys[2]));
                                s.Text = descifrado;
                            }


                        }
                    }
                }
            } 
            return View(chatRoom);
            

        }

        public async Task<ActionResult> SearchMessage(string roomId, string searched)
        {
            var decypherSDES = new SDES();
            var decypherRSA = new RSA();
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/allchats");
            var result = res.Content.ReadAsStringAsync().Result;
            var allChatRooms = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result);
            var chatRoom = new ChatRoom();
            List<string> mensajesdescifrados = new List<string>();
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            var currentUser = new Users();
            var keys = new Diffie_Hellman();
            ViewData["roomId"] = roomId;
            foreach (ChatRoom chat in allChatRooms)
            {
                if (chat.id.ToString() == roomId)
                {
                    //aqui
                    chatRoom = chat;
                    //aqui se descifran los mensajes
                    if (chatRoom.GroupName == null)
                    {

                        foreach (Messages s in chatRoom.messagesList)
                        {
                            if (s.isFile == false)
                            {

                                string descifrado = decypherSDES.Decypher(keys.getprivatekey(chatRoom.keys), s.Text);
                                s.Text = descifrado;

                            }


                        }
                    }
                    else
                    {
                        foreach (Messages s in chatRoom.messagesList)
                        {

                            if (s.isFile == false)
                            {
                                string descifrado = decypherRSA.RSA_DECYPHER(s.Text, Convert.ToInt32(chatRoom.keys[0]), Convert.ToInt32(chatRoom.keys[2]));
                                s.Text = descifrado;
                            }


                        }
                    }
                }


            }
            string user;
            if (chatRoom.chatMembers.Count == 2 && chatRoom.GroupName == null)
            {
                user = chatRoom.chatMembers.Find(x => x != ViewBag.sessionv);
            }
            else
            {
                user = chatRoom.GroupName;
            }
            ViewData["ChatWith"] = user;
        
            ViewData["GetMessage"] = searched;
            var messageRequest = from x in chatRoom.messagesList select x;
            if (!String.IsNullOrEmpty(searched))
            {

                //DELEGATES
              
                    messageRequest = messageRequest.Where(x => x.Text.Contains(searched));
                
           

            }
            return (View(messageRequest));
        }
        public async Task<IActionResult> SendMessage(string textMessage, string roomId)
        {

            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/allchats");
            var result = res.Content.ReadAsStringAsync().Result;
            var allChatRooms = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result);
            var chatRoom = new ChatRoom();
            var cifradoSDES = new SDES();
            var cifradoRSA = new RSA();
            var key = new Diffie_Hellman();
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            foreach (ChatRoom chat in allChatRooms)
            {
                if (chat.id.ToString() == roomId)
                {
                    chatRoom = chat;
                }
            }
            if (textMessage == null)
            {
                string EXIT;
                //verificar
                if (chatRoom.chatMembers.Count == 2 && chatRoom.GroupName == null)
                {
                    EXIT = chatRoom.chatMembers.Find(x => x != ViewBag.sessionv);
                }
                else
                {
                    EXIT = chatRoom.GroupName;
                }
                return RedirectToAction(nameof(Room), new { id = EXIT });
            }
            //CREAR MENSAJE DE TEXTO
            var newMessage = new Messages();
            newMessage.id = Guid.NewGuid().ToString();
            newMessage.Readers = chatRoom.chatMembers;
            
            newMessage.SenderUsername = ViewBag.sessionv;

            //newMessage.Text = textMessage;
            //aqui

            if (chatRoom.GroupName == null)
            {
                newMessage.Text = cifradoSDES.Cypher(key.getprivatekey(chatRoom.keys), textMessage);
            }
            else
            {
                newMessage.Text = cifradoRSA.RSA_CYPHER(textMessage, Convert.ToInt32(chatRoom.keys[0]), Convert.ToInt32(chatRoom.keys[1]));
            }


            newMessage.date = DateTime.Now.ToString();

            //aqui se deberia mandar a cifrar
            chatRoom.messagesList.Add(newMessage);
            //actualizar room en mongo
            var json = System.Text.Json.JsonSerializer.Serialize(chatRoom);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/user/chats/" + chatRoom.id , content);

            //ACCIONES PARA RETORNAR A LA VISTA DEL CHAT
            string reciever;
            //verificar
            if(chatRoom.chatMembers.Count == 2 && chatRoom.GroupName ==null)
            {
                reciever = chatRoom.chatMembers.Find(x => x != ViewBag.sessionv);
            }
            else
            {
                reciever = chatRoom.GroupName;
            }
            return  RedirectToAction(nameof(Room), new { id = reciever});
    
        }

        [HttpGet]
        public ActionResult UploadFile(string roomId, string sender)
        {
            ViewData["roomId"] = roomId;
            ViewData["sender"] = sender;

            return View();
        }

        


        [HttpPost]
        public async Task<IActionResult> UploadFile(string roomId, string sender, IFormFile File)
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/allchats");
            var result = res.Content.ReadAsStringAsync().Result;
            var allChatRooms = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result);
            var chatRoom = new ChatRoom();
            var LZWcompresor = new LZW();

            foreach(ChatRoom chat in allChatRooms)
            {
                if(chat.id == roomId)
                {
                    chatRoom = chat;
                    break;
                }
            }
         
            var newMsg = new Messages();
            newMsg.id = Guid.NewGuid().ToString();
            newMsg.Readers = chatRoom.chatMembers;
            newMsg.SenderUsername = sender;
            newMsg.isFile = true;
            newMsg.FileName = File.FileName;
            newMsg.ContentType = File.ContentType;
            newMsg.Text = "file";
            byte[] bytes;
            using (var target = new MemoryStream())
            {
                File.CopyTo(target);
                bytes = target.ToArray();
            }
            newMsg.File = LZWcompresor.Compress(bytes);
            chatRoom.messagesList.Add(newMsg);

            var json = System.Text.Json.JsonSerializer.Serialize(chatRoom);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync("api/user/chats/" +chatRoom.id, content);

   
              
            return RedirectToAction(nameof(Download), new { roomId = chatRoom.id });
        }

        [HttpGet]

        public async Task<ActionResult> Download(string roomId)
        {
            ViewData["roomId"] = roomId;
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/allchats");
            var result = res.Content.ReadAsStringAsync().Result;
            var allChatRooms = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result);
            var chatRoom = new ChatRoom();
            foreach (ChatRoom chat in allChatRooms)
            {
                if (chat.id == roomId)
                {
                    chatRoom = chat;
                    break;
                }
            }
            string user;
            if (chatRoom.chatMembers.Count == 2 && chatRoom.GroupName == null)
            {
                user = chatRoom.chatMembers.Find(x => x != ViewBag.sessionv);
            }
            else
            {
                user = chatRoom.GroupName;
            }
            ViewData["ChatWith"] = user;
            return View(chatRoom);
        }

      
        public async Task<IActionResult> DownloadFile(string roomId, string msgid)
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/allchats");
            var result = res.Content.ReadAsStringAsync().Result;
            var allChatRooms = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result);
            var chatRoom = new ChatRoom();
            var LZWcompresor = new LZW();
            var message = new Messages();
            var largodiccionario = new List<Byte>();
            var chardiccionario = new List<Byte>();
            var mensajecomprimido = new List<Byte>();


            foreach (ChatRoom chat in allChatRooms)
            {
                if (chat.id.ToString() == roomId)
                {
                    //aqui
                    chatRoom = chat;
                    //aqui se descifran los mensajes
                   
                        foreach (Messages s in chatRoom.messagesList)
                        {
                            if (s.id == msgid)
                            {

                            message = s;
                            break;

                            }


                        }
                    break;
               
                }
            }


            byte[] bytes = message.File;
            int i = 0;




            while (bytes[i] != 0)
            {

                largodiccionario.Add(bytes[i]);
                i++;

            }
            i++;
            int largo = 0;
            foreach (byte c in largodiccionario)
            {
                largo += Convert.ToInt32(c);
            }
            int z = 0;
            while (largo != z)
            {

                chardiccionario.Add(bytes[i]);
                z++;
                i++;

            }

            while (i != bytes.Length)
            {
                mensajecomprimido.Add(bytes[i]);
                i++;
            }

          
          


            byte[] mensajcomprimido = LZWcompresor.Decompress(chardiccionario.ToArray(), mensajecomprimido.ToArray());

            return new FileContentResult(mensajcomprimido, message.ContentType)
            {

                FileDownloadName = $"{message.FileName}"
            };
        }
        public async Task<IActionResult> DeleteForMe(string roomId, string msgId)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/allchats");
            var result = res.Content.ReadAsStringAsync().Result;
            var allChatRooms = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result);
            var chatRoom = new ChatRoom();
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            foreach (ChatRoom chat in allChatRooms)
            {
                if (chat.id.ToString() == roomId)
                {
                    chatRoom = chat;

                }
            }
            foreach( Messages message in chatRoom.messagesList)
            {
                if(message.id == msgId)
                {
                    message.Readers.Remove(ViewBag.sessionv);
                }
            }
            var json = System.Text.Json.JsonSerializer.Serialize(chatRoom);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            res = await client.PutAsync("api/user/chats/" + chatRoom.id, content);
            //ACCIONES PARA RETORNAR A LA VISTA DEL CHAT
            string reciever;
            if (chatRoom.chatMembers.Count == 2 && chatRoom.GroupName == null)
            {
                reciever = chatRoom.chatMembers.Find(x => x != ViewBag.sessionv);
            }
            else
            {
                reciever = chatRoom.GroupName;
            }
            return RedirectToAction(nameof(Room), new { id = reciever });


        }

        public async Task<IActionResult> DeleteForAll(string roomId, string msgId)
        {
            HttpClient client = _api.Initial();
            HttpResponseMessage res = await client.GetAsync("api/user/allchats");
            var result = res.Content.ReadAsStringAsync().Result;
            var allChatRooms = System.Text.Json.JsonSerializer.Deserialize<List<ChatRoom>>(result);
            var chatRoom = new ChatRoom();
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            foreach (ChatRoom chat in allChatRooms)
            {
                if (chat.id.ToString() == roomId)
                {
                    chatRoom = chat;

                }
            }
            foreach (Messages message in chatRoom.messagesList)
            {
                if (message.id == msgId)
                {
                    message.Readers.Clear();
                }
            }
            var json = System.Text.Json.JsonSerializer.Serialize(chatRoom);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            res = await client.PutAsync("api/user/chats/" + chatRoom.id, content);
            //ACCIONES PARA RETORNAR A LA VISTA DEL CHAT
            string reciever;
            if (chatRoom.chatMembers.Count == 2 && chatRoom.GroupName == null)
            {
                reciever = chatRoom.chatMembers.Find(x => x != ViewBag.sessionv);
            }
            else
            {
                reciever = chatRoom.GroupName;
            }
            return RedirectToAction(nameof(Room), new { id = reciever });


        }




        public void ShowDialog(string message)
        {
            TempData["alertMessage"] = message;

        }
    }
}
