using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Models;

namespace TransrodenProyecto.Controllers
{
    public class CargasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Cargas
        public ActionResult Index()
        {
            var cargas = db.Cargas.Include(c => c.Usuario);
            return View(cargas.ToList());
        }

        // GET: Cargas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carga carga = db.Cargas.Find(id);
            if (carga == null)
            {
                return HttpNotFound();
            }
            return View(carga);
        }





        // GET: Cargas/Create
        public ActionResult Create()
        {

            // Solo usuarios Transportistas
            ViewBag.Id_Usuario = new SelectList(db.Usuarios.Where(u => u.Rol == Rol.Transportista), "Id_Usuario", "Nombre");
            return View();

        }



        // POST: Cargas/Create


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Carga,Id_Usuario,Descripcion")] Carga carga)
        {
            // Verificar si la sesión contiene la información del usuario
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            // Obtener el usuario
            var usuarioId = (int)Session["UsuarioId"];
            var usuarioRol = (Rol)Session["UsuarioRol"];
            var sede = (int?)Session["Sede"];

            if (usuarioRol != Rol.Bodeguero)
            {
                ModelState.AddModelError("", "Solo los usuarios bodegueros pueden crear cargas.");
                ViewBag.Id_Usuario = new SelectList(db.Usuarios.Where(u => u.Rol == Rol.Transportista), "Id_Usuario", "Nombre", carga.Id_Usuario);
                return View(carga);
            }


            if (ModelState.IsValid)
            {

                carga.fecha_creacion = DateTime.Now;

                // Estado de la sede del usuario bodeguero
                if (sede.HasValue && (Sede)sede.Value == Sede.PerezZeledon)
                {
                    carga.Estado = EstadoCarga.BodegaPZ;
                    carga.Origen = OrigenCarga.PerezZeledon;
                    db.Cargas.Add(carga);
                    db.SaveChanges();
                    return RedirectToAction("AsignarPaquetePZCarga", "PaquetesCargasController");
                }
                else if (sede.HasValue && (Sede)sede.Value == Sede.SanJose)
                {
                    carga.Estado = EstadoCarga.BodegaSJ;
                    carga.Origen = OrigenCarga.SanJose;
                    db.Cargas.Add(carga);
                    db.SaveChanges();
                    return RedirectToAction("AsignarPaqueteSJCarga", "PaquetesCargasController");
                }    
            }


            ViewBag.Id_Usuario = new SelectList(db.Usuarios.Where(u => u.Rol == Rol.Transportista), "Id_Usuario", "Nombre", carga.Id_Usuario);
            return View(carga);
        }



        // GET: Cargas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carga carga = db.Cargas.Find(id);
            if (carga == null)
            {
                return HttpNotFound();
            }
            /*ViewBag.Id_Usuario = new SelectList(db.Usuarios, "Id_Usuario", "Nombre", carga.Id_Usuario);*/
            ViewBag.Id_Usuario = new SelectList(db.Usuarios.Where(u => u.Rol == Rol.Transportista), "Id_Usuario", "Nombre");
            return View(carga);
        }


        // POST: Cargas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Carga,Id_Usuario,Descripcion,NumeroPaquetes,Estado,Origen,fecha_creacion")] Carga carga)
        {

            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            // Obtener el usuario
            var usuarioId = (int)Session["UsuarioId"];
            var usuarioRol = (Rol)Session["UsuarioRol"];
            var sede = (int?)Session["Sede"];

            if (usuarioRol != Rol.Bodeguero)
            {
                ModelState.AddModelError("", "Solo los usuarios bodegueros pueden editar cargas.");
                ViewBag.Id_Usuario = new SelectList(db.Usuarios.Where(u => u.Rol == Rol.Transportista), "Id_Usuario", "Nombre", carga.Id_Usuario);
                return View(carga);
            }


            if (ModelState.IsValid)
            {
                // Estado de la sede del usuario bodeguero
                if (sede.HasValue && (Sede)sede.Value == Sede.PerezZeledon)
                {
                    db.Entry(carga).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("AsignarPaquetePZCarga", "PaquetesCargasController");
                }
                else if (sede.HasValue && (Sede)sede.Value == Sede.SanJose)
                {
                    db.Entry(carga).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("AsignarPaqueteSJCarga", "PaquetesCargasController");
                }
                
            }
            ViewBag.Id_Usuario = new SelectList(db.Usuarios, "Id_Usuario", "Nombre", carga.Id_Usuario);
            return View(carga);


        }





        // GET: Cargas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carga carga = db.Cargas.Find(id);
            if (carga == null)
            {
                return HttpNotFound();
            }
            return View(carga);
        }

        // POST: Cargas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Carga carga = db.Cargas.Include(c => c.Paquetes).FirstOrDefault(c => c.Id_Carga == id);


            if (carga.Paquetes != null && carga.Paquetes.Any())
            {
                ModelState.AddModelError("", "Debes quitar los paquetes asociados antes de eliminar esta carga!!!");
                return Redirect(Request.UrlReferrer.ToString());
            }


            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            // Obtener el usuario
            var usuarioId = (int)Session["UsuarioId"];
            var usuarioRol = (Rol)Session["UsuarioRol"];
            var sede = (int?)Session["Sede"];


            if (usuarioRol != Rol.Bodeguero)
            {
                ModelState.AddModelError("", "Solo los usuarios bodegueros pueden editar cargas.");
                ViewBag.Id_Usuario = new SelectList(db.Usuarios.Where(u => u.Rol == Rol.Transportista), "Id_Usuario", "Nombre", carga.Id_Usuario);
                return View(carga);
            }


            if (ModelState.IsValid)
            {
                // Estado de la sede del usuario bodeguero
                if (sede.HasValue && (Sede)sede.Value == Sede.PerezZeledon)
                {
                    db.Cargas.Remove(carga);
                    db.SaveChanges();
                    return RedirectToAction("AsignarPaquetePZCarga", "PaquetesCargasController");
                }
                else if (sede.HasValue && (Sede)sede.Value == Sede.SanJose)
                {
                    db.Cargas.Remove(carga);
                    db.SaveChanges();
                    return RedirectToAction("AsignarPaqueteSJCarga", "PaquetesCargasController");
                }

            }
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
    }
}
