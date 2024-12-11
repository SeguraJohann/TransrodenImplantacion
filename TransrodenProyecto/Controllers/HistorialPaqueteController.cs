using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TransrodenProyecto.Models;
using TransrodenProyecto.ViewModels;

namespace TransrodenProyecto.Controllers
{
    public class HistorialPaqueteController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Rastreo(string numeroRastreo)
        {

            if (string.IsNullOrEmpty(numeroRastreo))
            {
                return View(new HistorialPaqueteViewModel());
            }

            var paquete = db.Paquetes.FirstOrDefault(p => p.NumeroRastreo == numeroRastreo);

            if (paquete == null)
            {
                ModelState.AddModelError("", "Paquete no encontrado.");
                return View(new HistorialPaqueteViewModel()); // Vuelve a mostrar solo la barra de búsqueda con mensaje de error
            }

            var historialRastreo = db.Historiales
                .Where(r => r.NumeroRastreo == numeroRastreo)
                .OrderBy(r => r.Fecha)
                .ToList();

            var viewModel = new HistorialPaqueteViewModel
            {
                Paquete = paquete,
                Historial = historialRastreo
            };

            return View(viewModel);
        }



    }
}