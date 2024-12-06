using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Calculadora;
using TransrodenProyecto.Models;

namespace TransrodenProyecto.Controllers
{
    public class CalcDomiciliosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CalcDomicilios
        public ActionResult Index()
        {
            return View(db.CalcDomicilios.ToList());
        }

        // GET: CalcDomicilios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalcDomicilio calcDomicilio = db.CalcDomicilios.Find(id);
            if (calcDomicilio == null)
            {
                return HttpNotFound();
            }
            return View(calcDomicilio);
        }

        // GET: CalcDomicilios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CalcDomicilios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Domicilio,Nombre,Costo")] CalcDomicilio calcDomicilio)
        {
            if (ModelState.IsValid)
            {
                db.CalcDomicilios.Add(calcDomicilio);
                db.SaveChanges();
                return RedirectToAction("AdminCalculadora", "AdminCalculadora");
            }

            return RedirectToAction("AdminCalculadora", "AdminCalculadora");
        }

        // GET: CalcDomicilios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalcDomicilio calcDomicilio = db.CalcDomicilios.Find(id);
            if (calcDomicilio == null)
            {
                return HttpNotFound();
            }
            return View(calcDomicilio);
        }

        // POST: CalcDomicilios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Domicilio,Nombre,Costo")] CalcDomicilio calcDomicilio)
        {
            if (ModelState.IsValid)
            {
                db.Entry(calcDomicilio).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminCalculadora", "AdminCalculadora");
            }
            return RedirectToAction("AdminCalculadora", "AdminCalculadora");
        }

        // GET: CalcDomicilios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalcDomicilio calcDomicilio = db.CalcDomicilios.Find(id);
            if (calcDomicilio == null)
            {
                return HttpNotFound();
            }
            return View(calcDomicilio);
        }

        // POST: CalcDomicilios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CalcDomicilio calcDomicilio = db.CalcDomicilios.Find(id);
            db.CalcDomicilios.Remove(calcDomicilio);
            db.SaveChanges();
            return RedirectToAction("AdminCalculadora", "AdminCalculadora");
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
