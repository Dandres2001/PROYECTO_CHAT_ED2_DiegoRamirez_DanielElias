using Microsoft.AspNetCore.Http;
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
            try
            {

                var user = new Users();
                {

                    user.Username = collection["Username"];
                    user.Password = collection["Password"];
                    user.eMail = collection["eMail"];
                    user.friendsList = new List<string>();
                    user.requestsList = new List<string>();
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





                return RedirectToAction(nameof(Index));
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
        public ActionResult Chat()
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            return View();
        }

        //ACCIONES PARA AGREGAR CONTACTOS
        [HttpGet]
        public async Task<IActionResult> AddFriend(string searched)
        {

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
        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
            {
                return View();
            }

            // POST: UserController/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit(int id, IFormCollection collection)
            {
                try
                {
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public void ShowDialog(string message)
        {
            TempData["alertMessage"] = message;

        }
    }
}
