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
        // POST: Cuenta/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "Id_Usuario,Nombre,Apellidos,Cedula,Correo,Clave,Telefono,NotifCli")] Usuario usuarioActualizado)
        {
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

            // Actualizar los campos
            usuarioExistente.Nombre = usuarioActualizado.Nombre;
            usuarioExistente.Apellidos = usuarioActualizado.Apellidos;
            usuarioExistente.Cedula = usuarioActualizado.Cedula.Replace(" ", ""); // Eliminar espacios en cédula
            usuarioExistente.Correo = usuarioActualizado.Correo;
            usuarioExistente.Telefono = usuarioActualizado.Telefono.Replace(" ", ""); // Eliminar espacios en teléfono
            usuarioExistente.NotifCli = usuarioActualizado.NotifCli;

            // Manejar la actualización de la contraseña
            if (!string.IsNullOrEmpty(usuarioActualizado.Clave))
            {
                usuarioExistente.Clave = ConvertirSha256(usuarioActualizado.Clave);
            }
            // Si la clave está vacía, no se hace nada y se mantiene la existente

            try
            {
                db.Entry(usuarioExistente).State = EntityState.Modified;
                db.SaveChanges();

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
            // Limpiar espacios en cédula y teléfono
            if (usuario.Cedula != null)
                usuario.Cedula = usuario.Cedula.Replace(" ", "");

            if (usuario.Telefono != null)
                usuario.Telefono = usuario.Telefono.Replace(" ", "");

            // Validación de formato de nombre y apellidos (solo letras)
            // Validación de formato de nombre (solo letras)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                ModelState.AddModelError("Nombre", "El nombre solo debe contener letras.");
            }
            // Validación de longitud del nombre
            else if (usuario.Nombre.Length < 2)
            {
                ModelState.AddModelError("Nombre", "El nombre debe tener al menos 2 caracteres.");
            }
            else if (usuario.Nombre.Length > 15)
            {
                ModelState.AddModelError("Nombre", "El nombre no debe exceder los 50 caracteres.");
            }

            // Validación de formato de apellidos (solo letras)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Apellidos, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                ModelState.AddModelError("Apellidos", "Los apellidos solo deben contener letras.");
            }
            // Validación de longitud de apellidos
            else if (usuario.Apellidos.Length < 2)
            {
                ModelState.AddModelError("Apellidos", "Los apellidos deben tener al menos 2 caracteres.");
            }
            else if (usuario.Apellidos.Length > 50)
            {
                ModelState.AddModelError("Apellidos", "Los apellidos no deben exceder los 50 caracteres.");
            }

            // Validación de formato de cédula (solo números)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Cedula, @"^\d+$"))
            {
                ModelState.AddModelError("Cedula", "La cédula solo debe contener números.");
            }
            else if (usuario.Cedula.Length != 9)
            {
                ModelState.AddModelError("Cedula", "La cédula debe contener exactamente 9 dígitos.");
            }

            // Verificar si el correo ya existe
            var existingUserByEmail = db.Usuarios.FirstOrDefault(u => u.Correo == usuario.Correo);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Correo", "El correo ya está registrado.");
                return View(usuario);
            }

            // Verificar si la cédula ya existe
            var existingUserByCedula = db.Usuarios.FirstOrDefault(u => u.Cedula == usuario.Cedula);
            if (existingUserByCedula != null)
            {
                ModelState.AddModelError("Cedula", "Esta cédula ya está registrada en el sistema.");
                return View(usuario);
            }

            // Verificar si el teléfono ya existe
            var existingUserByTelefono = db.Usuarios.FirstOrDefault(u => u.Telefono == usuario.Telefono);
            if (existingUserByTelefono != null)
            {
                ModelState.AddModelError("Telefono", "Este número de teléfono ya está registrado en el sistema.");
                return View(usuario);
            }

            // Validación de formato de correo
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("Correo", "El formato del correo electrónico no es válido.");
            }

            // Validación de formato de teléfono (solo números y exactamente 8 caracteres)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Telefono, @"^\d{8}$"))
            {
                ModelState.AddModelError("Telefono", "El teléfono debe contener 8 dígitos numéricos.");
            }

            // Validación de seguridad de contraseña (configurable con una bandera)
            // Validación de seguridad de contraseña (configurable con una bandera)
            bool pruebasModo = false; // Cambiar a false para activar validación de contraseña más estricta

            if (!pruebasModo)
            {
                // Validaciones completas de seguridad
                if (usuario.Clave.Length < 8)
                {
                    ModelState.AddModelError("Clave", "La contraseña debe tener al menos 8 caracteres.");
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Clave, @"[a-zA-Z]"))
                {
                    ModelState.AddModelError("Clave", "La contraseña debe contener al menos una letra.");
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Clave, @"\d"))
                {
                    ModelState.AddModelError("Clave", "La contraseña debe contener al menos un número.");
                }
            }
            else
            {
                // Modo pruebas: validación mínima
                if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Clave, @"[a-zA-Z]"))
                {
                    ModelState.AddModelError("Clave", "La contraseña debe contener al menos una letra.");
                }
            }

            if (ModelState.IsValid)
            {
                // Verificar si el correo ya existe
                var existingUser = db.Usuarios.FirstOrDefault(u => u.Correo == usuario.Correo);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Correo", "El correo ya está registrado.");
                    return View(usuario);
                }

                usuario.Clave = ConvertirSha256(usuario.Clave);
                usuario.Rol = Rol.Cliente;

                db.Usuarios.Add(usuario);
                db.SaveChanges();

                // Para evitar el redirecionamiento automático que confunde al usuario
                TempData["RegistroExitoso"] = "Usuario registrado exitosamente";
                return RedirectToAction("Login", "Cuenta");
            }

            // Si hay errores de validación, retornar a la vista con los errores
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