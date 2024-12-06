using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Models;
using System.Threading;

namespace TransrodenProyecto.Controllers
{
    public class CuentaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cuenta/Index
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login");
            }

            int usuarioId = (int)Session["UsuarioId"];
            Usuario usuario = db.Usuarios.Find(usuarioId);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(usuario);
        }

        // POST: Cuenta/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id_Usuario,Nombre,Apellidos,Cedula,Correo,Clave,Telefono,NotifCli")] Usuario usuarioActualizado)
        {
            // Remover la validación de la clave
            ModelState.Remove("Clave");

            if (!ModelState.IsValid)
            {
                return View(usuarioActualizado);
            }

            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login");
            }

            int usuarioId = (int)Session["UsuarioId"];
            var usuarioExistente = db.Usuarios.Find(usuarioId);

            if (usuarioExistente == null)
            {
                return HttpNotFound();
            }

            // Si no se proporcionó una nueva clave, mantener la existente
            if (string.IsNullOrEmpty(usuarioActualizado.Clave))
            {
                usuarioActualizado.Clave = usuarioExistente.Clave; // Mantener la clave existente
            }
            else
            {
                usuarioActualizado.Clave = ConvertirSha256(usuarioActualizado.Clave); // Encriptar la nueva clave
            }

            // Actualizar los demás campos si han cambiado
            if (usuarioActualizado.Nombre != usuarioExistente.Nombre)
                usuarioExistente.Nombre = usuarioActualizado.Nombre;

            if (usuarioActualizado.Apellidos != usuarioExistente.Apellidos)
                usuarioExistente.Apellidos = usuarioActualizado.Apellidos;

            if (usuarioActualizado.Cedula != usuarioExistente.Cedula)
                usuarioExistente.Cedula = usuarioActualizado.Cedula;

            if (usuarioActualizado.Correo != usuarioExistente.Correo)
                usuarioExistente.Correo = usuarioActualizado.Correo;

            if (usuarioActualizado.Telefono != usuarioExistente.Telefono)
                usuarioExistente.Telefono = usuarioActualizado.Telefono;

            usuarioExistente.NotifCli = usuarioActualizado.NotifCli;

            try
            {
                db.Entry(usuarioExistente).State = EntityState.Modified;
                db.SaveChanges();

                // Actualizar el nombre en la sesión
                Session["Usuario"] = $"{usuarioExistente.Nombre}";

                TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al actualizar el perfil. Por favor, inténtelo de nuevo.");
                return View(usuarioActualizado);
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Usuarios.FirstOrDefault(u => u.Correo == usuario.Correo);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "El correo ya está registrado.");
                    return View(usuario);
                }

                usuario.Clave = ConvertirSha256(usuario.Clave);
                usuario.Rol = Rol.Cliente;

                db.Usuarios.Add(usuario);
                db.SaveChanges();
                Thread.Sleep(3000);

                return RedirectToAction("Login", "Cuenta");
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string correo, string clave)
        {
            var usuario = db.Usuarios.FirstOrDefault(u => u.Correo == correo);

            if (usuario != null)
            {
                if (usuario.LockoutEnabled && usuario.LockoutEnd.HasValue && usuario.LockoutEnd.Value > DateTime.Now)
                {
                    ModelState.AddModelError("", "Su cuenta está bloqueada. Intente de nuevo más tarde.");
                    return View();
                }

                string ClaveEncryt = ConvertirSha256(clave);

                if (usuario.Clave == ClaveEncryt)
                {
                    usuario.AccessFailedCount = 0;
                    usuario.LockoutEnd = null;
                    db.SaveChanges();

                    Session["UsuarioId"] = usuario.Id_Usuario;
                    Session["UsuarioRol"] = usuario.Rol;
                    Session["Usuario"] = $"{usuario.Nombre}";

                    //Esto es por si el usuario no tiene sede
                    if (usuario.Sede != null)
                    {
                        Session["Sede"] = usuario.Sede;
                    }
                    else
                    {
                        Session["Sede"] = Sede.SanJose; // Valor por defecto
                    }

                    // Almacenar la sede si el usuario es Bodeguero
                    if (usuario.Rol == Rol.Bodeguero)
                    {
                        Session["Sede"] = usuario.Sede;
                    }

                    if (usuario.Rol == Rol.Administrador || usuario.Rol == Rol.Bodeguero)
                    {
                        return RedirectToAction("IndexAdmin", "Admin");
                    }
                    else if (usuario.Rol == Rol.Cliente)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (usuario.Rol == Rol.Transportista)
                    {
                        return RedirectToAction("IndexTransportista", "Admin");
                    }
                }
                else
                {
                    usuario.AccessFailedCount++;

                    if (usuario.AccessFailedCount >= 2)
                    {
                        usuario.LockoutEnd = DateTime.Now.AddMinutes(1);
                    }
                    db.SaveChanges();

                    ModelState.AddModelError("", "Correo o clave incorrectos.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Correo o clave incorrectos.");
            }

            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Cuenta");
        }

        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}