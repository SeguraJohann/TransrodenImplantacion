using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Models;
using TransrodenProyecto.Security;

namespace TransrodenProyecto.Controllers
{
    public class UsuariosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Usuarios
        [AdminOnly]
        public ActionResult Index(string searchCed)
        {

            //Esto es una asignacion de pesos para ordenar, siendo el cliente con el peso mas alto lo que lo hace ultimo en listarse
            var usuarios = db.Usuarios.ToList()
                .OrderBy(u => u.Rol == Rol.Cliente ? 4 :
                              u.Rol == Rol.Transportista ? 3 :
                              u.Rol == Rol.Bodeguero ? 2 : 1)
                .ToList();

            if (!string.IsNullOrEmpty(searchCed))
            {
                usuarios = usuarios.Where(u => u.Cedula.Contains(searchCed)).ToList();
            }


            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        [AdminOnly]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // GET: Usuarios/Create
        [AdminOnly]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Usuario,Nombre,Apellidos,Cedula,Correo,Clave,Telefono,Rol,Sede,NotifCli")] Usuario usuario)
        {
            // Limpiar espacios en cédula y teléfono
            if (usuario.Cedula != null)
                usuario.Cedula = usuario.Cedula.Replace(" ", "");

            if (usuario.Telefono != null)
                usuario.Telefono = usuario.Telefono.Replace(" ", "");

            // Validar longitud de nombre
            if (string.IsNullOrEmpty(usuario.Nombre) || usuario.Nombre.Length < 2)
            {
                ModelState.AddModelError("Nombre", "El nombre debe tener al menos 2 caracteres.");
            }

            // Validar formato de nombre (solo letras)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                ModelState.AddModelError("Nombre", "El nombre solo debe contener letras.");
            }

            // Validar longitud de apellidos
            if (string.IsNullOrEmpty(usuario.Apellidos) || usuario.Apellidos.Length < 2)
            {
                ModelState.AddModelError("Apellidos", "Los apellidos deben tener al menos 2 caracteres.");
            }

            // Validar formato de apellidos (solo letras)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Apellidos, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                ModelState.AddModelError("Apellidos", "Los apellidos solo deben contener letras.");
            }

            // Validar formato de cédula (solo números, 9 caracteres)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Cedula, @"^\d{9}$"))
            {
                ModelState.AddModelError("Cedula", "La cédula debe contener 9 dígitos numéricos.");
            }
            else
            {
                // Verificar si la cédula ya existe
                var existingUserByCedula = db.Usuarios.FirstOrDefault(u => u.Cedula == usuario.Cedula);
                if (existingUserByCedula != null)
                {
                    ModelState.AddModelError("Cedula", "Esta cédula ya está registrada en el sistema.");
                }
            }

            // Validar formato de correo
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("Correo", "El formato del correo electrónico no es válido.");
            }
            else
            {
                // Verificar si el correo ya existe
                var existingUserByEmail = db.Usuarios.FirstOrDefault(u => u.Correo == usuario.Correo);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Correo", "El correo ya está registrado.");
                }
            }

            // Validar formato de teléfono (solo números, 8 caracteres)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Telefono, @"^\d{8}$"))
            {
                ModelState.AddModelError("Telefono", "El teléfono debe contener 8 dígitos numéricos.");
            }
            else
            {
                // Verificar si el teléfono ya existe
                var existingUserByTelefono = db.Usuarios.FirstOrDefault(u => u.Telefono == usuario.Telefono);
                if (existingUserByTelefono != null)
                {
                    ModelState.AddModelError("Telefono", "Este número de teléfono ya está registrado en el sistema.");
                }
            }

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
                if (string.IsNullOrEmpty(usuario.Clave))
                {
                    ModelState.AddModelError("Clave", "La contraseña no puede estar vacía.");
                }
            }

            if (ModelState.IsValid)
            {
                usuario.Clave = ConvertirSha256(usuario.Clave);
                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }
        // GET: Usuarios/Edit/5
        [AdminOnly]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Guardar la contraseña encriptada en ViewBag para usarla si no se modifica
            ViewBag.ClaveEncriptada = usuario.Clave;

            // Se envía con clave vacía a la vista
            usuario.Clave = string.Empty;

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Usuario,Nombre,Apellidos,Cedula,Correo,Clave,Telefono,Rol,Sede,NotifCli")] Usuario usuario, string claveEncriptada)
        {
            // Limpiar espacios en cédula y teléfono
            if (usuario.Cedula != null)
                usuario.Cedula = usuario.Cedula.Replace(" ", "");

            if (usuario.Telefono != null)
                usuario.Telefono = usuario.Telefono.Replace(" ", "");

            // Eliminar validación del ModelState para la clave si está vacía
            // Esto permite ignorar la anotación [Required] del modelo
            if (string.IsNullOrEmpty(usuario.Clave))
            {
                ModelState.Remove("Clave");
            }

            // Validar longitud de nombre
            if (string.IsNullOrEmpty(usuario.Nombre) || usuario.Nombre.Length < 2)
            {
                ModelState.AddModelError("Nombre", "El nombre debe tener al menos 2 caracteres.");
            }

            // Validar formato de nombre (solo letras)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Nombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                ModelState.AddModelError("Nombre", "El nombre solo debe contener letras.");
            }

            // Validar longitud de apellidos
            if (string.IsNullOrEmpty(usuario.Apellidos) || usuario.Apellidos.Length < 2)
            {
                ModelState.AddModelError("Apellidos", "Los apellidos deben tener al menos 2 caracteres.");
            }

            // Validar formato de apellidos (solo letras)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Apellidos, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                ModelState.AddModelError("Apellidos", "Los apellidos solo deben contener letras.");
            }

            // Validar formato de cédula (solo números, 9 caracteres)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Cedula, @"^\d{9}$"))
            {
                ModelState.AddModelError("Cedula", "La cédula debe contener 9 dígitos numéricos.");
            }
            else
            {
                // Verificar si la cédula ya existe en otro usuario
                var existingUserByCedula = db.Usuarios.FirstOrDefault(u => u.Cedula == usuario.Cedula && u.Id_Usuario != usuario.Id_Usuario);
                if (existingUserByCedula != null)
                {
                    ModelState.AddModelError("Cedula", "Esta cédula ya está registrada por otro usuario.");
                }
            }

            // Validar formato de correo
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("Correo", "El formato del correo electrónico no es válido.");
            }
            else
            {
                // Verificar si el correo ya existe en otro usuario
                var existingUserByEmail = db.Usuarios.FirstOrDefault(u => u.Correo == usuario.Correo && u.Id_Usuario != usuario.Id_Usuario);
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Correo", "El correo ya está registrado por otro usuario.");
                }
            }

            // Validar formato de teléfono (solo números, 8 caracteres)
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Telefono, @"^\d{8}$"))
            {
                ModelState.AddModelError("Telefono", "El teléfono debe contener 8 dígitos numéricos.");
            }
            else
            {
                // Verificar si el teléfono ya existe en otro usuario
                var existingUserByTelefono = db.Usuarios.FirstOrDefault(u => u.Telefono == usuario.Telefono && u.Id_Usuario != usuario.Id_Usuario);
                if (existingUserByTelefono != null)
                {
                    ModelState.AddModelError("Telefono", "Este número de teléfono ya está registrado por otro usuario.");
                }
            }

            if (ModelState.IsValid)
            {
                // Lógica de contraseña: Si está vacía, usar la original; si tiene valor, encriptar la nueva
                if (string.IsNullOrEmpty(usuario.Clave))
                {
                    // Mantener la contraseña original si no se ingresó una nueva
                    usuario.Clave = claveEncriptada;
                }
                else
                {
                    // Validación de seguridad de contraseña
                    bool pruebasModo = false; // Cambiar a false para activar validación de contraseña más estricta
                    bool esContrasenaValida = true;

                    if (!pruebasModo)
                    {
                        // Validaciones completas de seguridad
                        if (usuario.Clave.Length < 8)
                        {
                            ModelState.AddModelError("Clave", "La contraseña debe tener al menos 8 caracteres.");
                            esContrasenaValida = false;
                        }

                        if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Clave, @"[a-zA-Z]"))
                        {
                            ModelState.AddModelError("Clave", "La contraseña debe contener al menos una letra.");
                            esContrasenaValida = false;
                        }

                        if (!System.Text.RegularExpressions.Regex.IsMatch(usuario.Clave, @"\d"))
                        {
                            ModelState.AddModelError("Clave", "La contraseña debe contener al menos un número.");
                            esContrasenaValida = false;
                        }
                    }

                    if (esContrasenaValida)
                    {
                        // Encriptar la nueva contraseña
                        usuario.Clave = ConvertirSha256(usuario.Clave);
                    }
                    else
                    {
                        // Si hay errores de validación en la contraseña
                        ViewBag.ClaveEncriptada = claveEncriptada;
                        return View(usuario);
                    }
                }

                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Si hay errores, mantener la clave encriptada original
            ViewBag.ClaveEncriptada = claveEncriptada;
            return View(usuario);
        }
        // GET: Usuarios/Delete/5
        [AdminOnly]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Usuario usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            // Verificar si es un administrador
            if (usuario.Rol == Rol.Administrador)
            {
                TempData["ErrorMessage"] = "No se puede eliminar un usuario con rol de Administrador.";
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuarios.Find(id);

            // Verificar nuevamente si es un administrador
            if (usuario.Rol == Rol.Administrador)
            {
                TempData["ErrorMessage"] = "No se puede eliminar un usuario con rol de Administrador.";
                return RedirectToAction("Index");
            }

            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public static string ConvertirSha256(string texto)
        {
            //using System.Text;
            //USAR LA REFERENCIA DE "System.Security.Cryptography"

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

        //Codigo para la busqueda y para el buscador
        public ActionResult GetUsuarios(int page = 1, string searchCed = null)
        {
            int pageSize = 10;

            // Filtra los usuarios si hay un criterio de búsqueda
            var usuariosQuery = db.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(searchCed))
            {
                usuariosQuery = usuariosQuery.Where(u => u.Cedula.Contains(searchCed) || u.Nombre.Contains(searchCed));
            }

            // Aplica el ordenamiento y paginación
            var usuarios = usuariosQuery
                .OrderBy(u => u.Rol == Rol.Cliente ? 4 :
                              u.Rol == Rol.Transportista ? 3 :
                              u.Rol == Rol.Bodeguero ? 2 : 1)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalUsuarios = usuariosQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalUsuarios / pageSize);

            return Json(new
            {
                data = usuarios,
                totalPages = totalPages
            }, JsonRequestBehavior.AllowGet);
        }
    }
}