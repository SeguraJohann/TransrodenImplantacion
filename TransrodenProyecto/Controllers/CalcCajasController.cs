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
    public class CalcCajasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CalcCajas
        public ActionResult Index()
        {
            return View(db.CalcCaja.ToList());
        }

        // GET: CalcCajas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalcCaja calcCaja = db.CalcCaja.Find(id);
            if (calcCaja == null)
            {
                return HttpNotFound();
            }
            return View(calcCaja);
        }

        // GET: CalcCajas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CalcCajas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Caja,Tipo,Costo")] CalcCaja calcCaja)
        {
            if (ModelState.IsValid)
            {
                db.CalcCaja.Add(calcCaja);
                db.SaveChanges();
                return RedirectToAction("AdminCalculadora", "AdminCalculadora");
            }

            return RedirectToAction("AdminCalculadora", "AdminCalculadora");
        }

        // GET: CalcCajas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalcCaja calcCaja = db.CalcCaja.Find(id);
            if (calcCaja == null)
            {
                return HttpNotFound();
            }
            return View(calcCaja);
        }

        // POST: CalcCajas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Caja,Tipo,Costo")] CalcCaja calcCaja)
        {
            if (ModelState.IsValid)
            {
                db.Entry(calcCaja).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminCalculadora", "AdminCalculadora");
            }
            return RedirectToAction("AdminCalculadora", "AdminCalculadora");
        }

        // GET: CalcCajas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalcCaja calcCaja = db.CalcCaja.Find(id);
            if (calcCaja == null)
            {
                return HttpNotFound();
            }
            return View(calcCaja);
        }

        // POST: CalcCajas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CalcCaja calcCaja = db.CalcCaja.Find(id);
            db.CalcCaja.Remove(calcCaja);
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
