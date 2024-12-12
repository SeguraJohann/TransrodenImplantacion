using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Models;
using TransrodenProyecto.Security;
using TransrodenProyecto.ViewModels;

namespace TransrodenProyecto.Controllers
{
    public class CamionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Camions
        [AdminOnly]
        public ActionResult Index()
        {
            var camiones = db.Camiones.Include(c => c.Usuario);
            return View(camiones.ToList());
        }

        [AdminOnly]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Obtén el camión
            Camion camion = db.Camiones.Find(id);
            if (camion == null)
            {
                return HttpNotFound();
            }


            var kilometrajes = db.Kilometrajes.Where(k => k.Id_Camion == id).OrderByDescending(k => k.Registro).ToList();


            CamionKmViewModel viewModel = new CamionKmViewModel
            {
                CamionActual = camion,
                Kilometros = kilometrajes
            };

            return View(viewModel);
        }



        // GET: Camions/Create
        [AdminOnly]
        public ActionResult Create()
        {

            var transportista = db.Usuarios.Where(u => u.Rol == Rol.Transportista).Select(u => new SelectListItem{ Value = u.Id_Usuario.ToString(), Text = u.Nombre + " " + u.Apellidos }).ToList();


            transportista.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Sin Transportista"
            });

            ViewBag.Id_Usuario = transportista;
            return View();
        }




        // POST: Camions/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Camion,Id_Usuario,Marca,Modelo,TipoCarga,Disponible")] Camion camion)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(camion.Id_Usuario.ToString()))
                {
                    camion.Id_Usuario = null;
                }

                db.Camiones.Add(camion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Por si no se selecciono un valor valido, se pone el valor de nulo
            var transportista = db.Usuarios .Where(u => u.Rol == Rol.Transportista).Select(u => new SelectListItem{ Value = u.Id_Usuario.ToString(), Text = u.Nombre + " " + u.Apellidos }).ToList();

            transportista.Insert(0, new SelectListItem{ Value = "", Text = "Sin Usuario" });



            ViewBag.Id_Usuario = transportista;
            return View(camion);
        }




        // GET: Camions/Edit/5
        [AdminOnly]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Camion camion = db.Camiones.Find(id);
            if (camion == null)
            {
                return HttpNotFound();
            }

            var transportista = db.Usuarios.Where(u => u.Rol == Rol.Transportista).Select(u => new SelectListItem { Value = u.Id_Usuario.ToString(), Text = u.Nombre + " " + u.Apellidos }).ToList();


            transportista.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Sin Transportista"
            });

            ViewBag.Id_Usuario = transportista;

            return View(camion);
        }



        // POST: Camions/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Camion,Id_Usuario,Marca,Modelo,TipoCarga,Disponible")] Camion camion)
        {
            if (ModelState.IsValid)
            {

                if (string.IsNullOrEmpty(camion.Id_Usuario.ToString()))
                {
                    camion.Id_Usuario = null;
                }

                db.Entry(camion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // Por si no se selecciono un valor valido, se pone el valor de nulo
            var transportista = db.Usuarios.Where(u => u.Rol == Rol.Transportista).Select(u => new SelectListItem { Value = u.Id_Usuario.ToString(), Text = u.Nombre + " " + u.Apellidos }).ToList();

            transportista.Insert(0, new SelectListItem { Value = "", Text = "Sin Usuario" });


            ViewBag.Id_Usuario = transportista;
            return View(camion);
        }




        // GET: Camions/Delete/5
        [AdminOnly]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Camion camion = db.Camiones.Find(id);
            if (camion == null)
            {
                return HttpNotFound();
            }
            return View(camion);
        }



        // POST: Camions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Camion camion = db.Camiones.Find(id);
            db.Camiones.Remove(camion);
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
    }
}
