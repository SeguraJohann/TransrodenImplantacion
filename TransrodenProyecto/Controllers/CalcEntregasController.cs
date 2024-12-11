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
    public class CalcEntregasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CalcEntregas
        public ActionResult Index()
        {
            return View(db.CalcEntrega.ToList());
        }

        // GET: CalcEntregas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalcEntrega calcEntrega = db.CalcEntrega.Find(id);
            if (calcEntrega == null)
            {
                return HttpNotFound();
            }
            return View(calcEntrega);
        }

        // GET: CalcEntregas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CalcEntregas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Entrega,Tipo,Tarifa")] CalcEntrega calcEntrega)
        {
            if (ModelState.IsValid)
            {
                db.CalcEntrega.Add(calcEntrega);
                db.SaveChanges();
                return RedirectToAction("AdminCalculadora", "AdminCalculadora");
            }

            return RedirectToAction("AdminCalculadora", "AdminCalculadora");
        }

        // GET: CalcEntregas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalcEntrega calcEntrega = db.CalcEntrega.Find(id);
            if (calcEntrega == null)
            {
                return HttpNotFound();
            }
            return View(calcEntrega);
        }

        // POST: CalcEntregas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Entrega,Tipo,Tarifa")] CalcEntrega calcEntrega)
        {
            if (ModelState.IsValid)
            {
                db.Entry(calcEntrega).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminCalculadora", "AdminCalculadora");
            }
            return RedirectToAction("AdminCalculadora", "AdminCalculadora");
        }

        // GET: CalcEntregas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalcEntrega calcEntrega = db.CalcEntrega.Find(id);
            if (calcEntrega == null)
            {
                return HttpNotFound();
            }
            return View(calcEntrega);
        }

        // POST: CalcEntregas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CalcEntrega calcEntrega = db.CalcEntrega.Find(id);
            db.CalcEntrega.Remove(calcEntrega);
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
