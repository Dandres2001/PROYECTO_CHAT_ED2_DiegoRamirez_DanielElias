using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Models;
using PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Repositories;

namespace PROYECTO_CHAT_ED2_DiegoRamirez_DanielElias.Controllers
{
    public class UserController : Controller
    {

        private IUserCollection db = new UserCollection();
        // GET: UserController
        public ActionResult Index()
        {
            var users = db.GetAllUsers();
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
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                List<Users> usersList = db.GetAllUsers().ToList();
                var user = new Users();
                {
                    user.guid = Guid.NewGuid().ToString();
                    user.Username = collection["Username"];
                    user.Password = collection["Password"];
                    user.eMail = collection["eMail"];
                    user.friendsList = new List<Users>();
                    user.requestsList = new List<Users>();
                };

                foreach(var u in usersList)
                {
                    if(u.Username == user.Username)
                    {
                        //NO PUEDE CREARLO
                        ShowDialog("El usuario que desea ya se encuentra ocupado");
                        return View();
                    }
                }
                db.createUser(user);

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
        public ActionResult Login(IFormCollection collection)
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

                foreach (var u in usersList)
                {
                    if (u.Username == user.Username && u.Password == user.Password)
                    {
                       
                        //ShowDialog("Sesión iniciada como: " + u.Username);
                        HttpContext.Session.SetString("usuarioLogeado", u.Username);
                        ViewBag.sessionv= HttpContext.Session.GetString("usuarioLogeado");
                        //RETORNAR ACCION DE SESION INICIADA
                        return RedirectToAction(nameof(Chat));
                    }
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
        public ActionResult AddFriend(string searched)
        {
            List<Users> usersList = db.GetAllUsers().ToList();
            ViewData["GetUser"] = searched;
            var userRequest = from x in usersList select x;
            if (!String.IsNullOrEmpty(searched))
            {

                //DELEGATES
                userRequest = userRequest.Where(x => x.Username.Contains(searched));

            }
            return View(userRequest);
        }

        public ActionResult SendRequest(string user)
        {
            List<Users> usersList = db.GetAllUsers().ToList();
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
           
            var sender = usersList.Find(x => x.Username == ViewBag.sessionv);
            usersList.Find(x => x.Username == user).requestsList.Add(sender);

            return RedirectToAction(nameof(Chat));
        }
        [HttpGet]
        public ActionResult ViewRequests()
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            List<Users> usersList = db.GetAllUsers().ToList();

            var user = new Users();
            foreach (var u in usersList)
            {
                if (u.Username == ViewBag.sessionv)
                {

                    user.guid = u.guid;
                    user.Username = u.Username;
                    user.Password = u.Password;
                    user.eMail = u.eMail;
                    user.friendsList = u.friendsList;
                    user.requestsList = u.requestsList;

                    break;

                }
            }

            return View(user.requestsList);
        }
        public ActionResult AcceptRequest(string user)
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            List<Users> usersList = db.GetAllUsers().ToList();
            var u = usersList.Find(x => x.Username == user);
            usersList.Find(x => x.Username == ViewBag.sessionv).requestsList.Remove(u);
            usersList.Find(x => x.Username == ViewBag.sessionv).friendsList.Add(u);
            usersList.Find(x => x.Username == user).friendsList.Add(ViewBag.sessionv);

            return RedirectToAction(nameof(ViewRequests));
        }
        public ActionResult DeclineRequest(string user)
        {
            ViewBag.sessionv = HttpContext.Session.GetString("usuarioLogeado");
            List<Users> usersList = db.GetAllUsers().ToList();
            var u = usersList.Find(x => x.Username == user);
            usersList.Find(x => x.Username == ViewBag.sessionv).requestsList.Remove(u);
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
