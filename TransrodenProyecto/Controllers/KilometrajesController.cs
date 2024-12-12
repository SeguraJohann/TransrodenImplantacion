using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Models;
using TransrodenProyecto.ViewModels;

namespace TransrodenProyecto.Controllers
{
    public class KilometrajesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Kilometrajes
        public ActionResult Index()
        {
            var kilometrajes = db.Kilometrajes.Include(k => k.Camion);
            return View(kilometrajes.ToList());
        }

        // GET: Kilometrajes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kilometraje kilometraje = db.Kilometrajes.Find(id);
            if (kilometraje == null)
            {
                return HttpNotFound();
            }
            return View(kilometraje);
        }

        // GET: Kilometrajes/Create
        public ActionResult Create()
        {
            ViewBag.Id_Camion = new SelectList(db.Camiones, "Id_Camion", "Marca");
            return View();
        }

        // POST: Kilometrajes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Km,Id_Camion,Km_registrado,Km_actual,Transportista,Registro")] Kilometraje kilometraje)
        {
            if (ModelState.IsValid)
            {
                db.Kilometrajes.Add(kilometraje);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_Camion = new SelectList(db.Camiones, "Id_Camion", "Marca", kilometraje.Id_Camion);
            return View(kilometraje);
        }

        // GET: Kilometrajes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kilometraje kilometraje = db.Kilometrajes.Find(id);
            if (kilometraje == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Camion = new SelectList(db.Camiones, "Id_Camion", "Marca", kilometraje.Id_Camion);
            return View(kilometraje);
        }

        // POST: Kilometrajes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Km,Id_Camion,Km_registrado,Km_actual,Transportista,Registro")] Kilometraje kilometraje)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kilometraje).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_Camion = new SelectList(db.Camiones, "Id_Camion", "Marca", kilometraje.Id_Camion);
            return View(kilometraje);
        }

        // GET: Kilometrajes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kilometraje kilometraje = db.Kilometrajes.Find(id);
            if (kilometraje == null)
            {
                return HttpNotFound();
            }
            return View(kilometraje);
        }

        // POST: Kilometrajes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kilometraje kilometraje = db.Kilometrajes.Find(id);
            db.Kilometrajes.Remove(kilometraje);
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


        // +++++++++++++++++++++++++++++++++++ Camiones Hecho +++++++++++++++++++++++++++++++++++++++++


        //Transportista
        public ActionResult MiCamion()
        {

            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            var usuarioId = (int)Session["UsuarioId"];
            var usuarioRol = (Rol)Session["UsuarioRol"];


            var camionActual = db.Camiones.FirstOrDefault(c => c.Id_Usuario == usuarioId);

            var camionesDisponibles = db.Camiones.Where(c => c.Id_Usuario == null).Select(c => new SelectListItem{ Value = c.Id_Camion.ToString(),Text = c.Marca + " - " + c.Modelo}).ToList();


            if (camionActual != null) 
            { 
                camionesDisponibles.Insert(0, new SelectListItem{ Value = camionActual.Id_Camion.ToString(),Text = camionActual.Marca + " - " + camionActual.Modelo + " (Actual)",Selected = true});
            }


            var viewModel = new CamionKmViewModel
            {

                Kilometros = camionActual != null ? db.Kilometrajes.Where(k => k.Id_Camion == camionActual.Id_Camion).OrderByDescending(k => k.Registro).ToList() : new List<Kilometraje>(), // Si no hay camión, lista vacía
                CamionActual = camionActual,
                CamionesDisponibles = camionesDisponibles
            };

            return View(viewModel);
        }


        //Transportista
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarCamion(int Id_Camion)
        {
            
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            var usuarioId = (int)Session["UsuarioId"];
            var usuarioRol = (Rol)Session["UsuarioRol"];



            var camionActual = db.Camiones.FirstOrDefault(c => c.Id_Usuario == usuarioId);


            if (camionActual != null)
            {
                camionActual.Id_Usuario = null;
            }


            var nuevoCamion = db.Camiones.Find(Id_Camion);
            if (nuevoCamion != null)
            {
                nuevoCamion.Id_Usuario = usuarioId;
            }


            db.SaveChanges();
            return RedirectToAction("MiCamion");
        }



        public ActionResult KmRegistro(int id)
        {
            // Obtener el camión por ID
            var camion = db.Camiones.Find(id);
            if (camion == null)
            {
                return HttpNotFound();
            }

            // Crear un nuevo registro de kilómetros relacionado con el camión
            var registro = new Kilometraje
            {
                Id_Camion = id,
                Registro = DateTime.Now
            };

            return View(registro);
        }



        //Transportista
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarKm(Kilometraje kilometros)
        {
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }
            var usuarioNombre = (string)Session["Usuario"];

            // Para obtener el ultimo registro
            var ultimoRegistro = db.Kilometrajes.Where(k => k.Id_Camion == kilometros.Id_Camion).OrderByDescending(k => k.Registro).FirstOrDefault();


            kilometros.Km_anterior = ultimoRegistro != null ? ultimoRegistro.Km_actual : 0; // Si no hay registro se pondra un 0
            kilometros.Transportista = usuarioNombre;
            kilometros.Registro = DateTime.Now;


            if (kilometros.Km_actual <= kilometros.Km_anterior)
            {
                ModelState.AddModelError("Km_actual", "El kilometraje ingresado debe ser mayor al kilometraje anterior.");
                return View("KmRegistro", kilometros); 
            }


            db.Kilometrajes.Add(kilometros);
            db.SaveChanges();

            return RedirectToAction("MiCamion");
        }


    }
}
