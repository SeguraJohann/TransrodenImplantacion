using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Models;
using TransrodenProyecto.ViewModels;

namespace TransrodenProyecto.Controllers
{
    public class AdminCalculadoraController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Admin
        public ActionResult AdminCalculadora()
        {

            var viewModel = new AdminCalculadoraViewModel
            {
                Domicilio = db.CalcDomicilios.ToList(),
                Caja = db.CalcCaja.ToList(),
                Entrega = db.CalcEntrega.ToList()
            };

            return View(viewModel);
        }
    }
}