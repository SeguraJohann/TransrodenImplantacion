using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Models;

namespace TransrodenProyecto.Controllers
{
    //Extension de DATETIME para ordenar fechas

    //Observacion importante la tabla Facturacions la detecta como Facturaciones!
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }

    // Controller 

    public class ContabilidadController : Controller

    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Contabilidad
        public ActionResult ContabilidadDiaria(DateTime? fecha)
        {
            IEnumerable<Facturacion> facturaciones;

            if (fecha.HasValue) 
            {
                // Filtrar las facturaciones según la fecha seleccionada
                facturaciones = db.Facturaciones
                    .Where(f => f.Fecha.Date == fecha.Value.Date)
                    .ToList();
            }
            else
            {
                // Si no se selecciona fecha, mostrar todas las facturaciones
                facturaciones = db.Facturaciones.ToList();
            }
            ViewBag.HayDatos = facturaciones.Any();
            return View();
        }

        public ActionResult ContabilidadSemanal(DateTime? fechaInicio)
        {
            if (fechaInicio == null)
            {
                // Si no se proporciona una fecha, usa la semana actual.
                fechaInicio = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            }

            // Calcula la fecha final de la semana (7 días después de la fecha de inicio)
            var fechaFin = fechaInicio.Value.AddDays(7);

            // Filtra las facturaciones que están dentro del rango de fechas
            var facturaciones = db.Facturaciones
                                  .Where(f => f.Fecha >= fechaInicio && f.Fecha < fechaFin)
                                  .ToList();

            // Agrupa las facturaciones por día de la semana
            var ContabilidadDiaria = facturaciones.GroupBy(f => f.Fecha.DayOfWeek)
                                                    .OrderBy(g => g.Key) // Ordena de lunes a domingo
                                                    .ToDictionary(g => g.Key, g => g.ToList());

            double totalSemanal = facturaciones.Sum(f => f.Total);

            ViewBag.ContabilidadDiaria = ContabilidadDiaria;
            ViewBag.FechaInicio = fechaInicio.Value;
            ViewBag.TotalSemanal = totalSemanal;
            return View();
        }



        public ActionResult ContabilidadMensual(int? mes, int? año)
        {
            if (mes == null || año == null)
            {
                // Si no se proporcionan el mes y el año, usa el mes y año actual.
                var fechaActual = DateTime.Now;
                mes = fechaActual.Month;
                año = fechaActual.Year;
            }

            // Filtra las facturaciones que están dentro del mes y año seleccionados
            var facturaciones = db.Facturaciones
                                  .Where(f => f.Fecha.Month == mes && f.Fecha.Year == año)
                                  .ToList();

            // Agrupa las facturaciones por día del mes
            var ContabilidadDiaria = facturaciones.GroupBy(f => f.Fecha.Day)
                                                    .OrderBy(g => g.Key)
                                                    .ToDictionary(g => g.Key, g => g.ToList());

            double totalMensual = facturaciones.Sum(f => f.Total);

            ViewBag.ContabilidadDiaria = ContabilidadDiaria;
            ViewBag.Mes = mes.Value;
            ViewBag.Año = año.Value;
            ViewBag.TotalMensual = totalMensual;
            return View();
        }



        public ActionResult ContabilidadAnual(int? yearCont)
        {
            if (yearCont == null)
            {
                // Si no se proporciona el año, usa el año actual.
                yearCont = DateTime.Now.Year;
            }

            // Filtra las facturaciones que están dentro del año seleccionado.
            var facturaciones = db.Facturaciones
                                  .Where(f => f.Fecha.Year == yearCont)
                                  .ToList();

            // Agrupa las facturaciones por mes
            var contabilidadMensual = facturaciones.GroupBy(f => f.Fecha.Month)
                                                    .OrderBy(g => g.Key)
                                                    .ToDictionary(g => g.Key, g => g.ToList());

            double totalAnual = facturaciones.Sum(f => f.Total);

            ViewBag.ContabilidadMensual = contabilidadMensual;
            ViewBag.YearCont = yearCont.Value;
            ViewBag.TotalAnual = totalAnual;
            return View();
        }











        //VISTA PARA CREAR DATOS EN CONTABILIDAD (SOLAMENTE PARA TESTEAR)

        public ActionResult Create()
        {
            // Si necesitas cargar datos relacionados para desplegarlos en un dropdown o similar, puedes cargarlos aquí.
            ViewBag.Id_Paquete = new SelectList(db.Paquetes, "Id", "Nombre"); // Ejemplo de carga de paquetes
            ViewBag.Id_Usuario = new SelectList(db.Usuarios, "Id", "Nombre"); // Ejemplo de carga de usuarios
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Facturacion facturacion)
        {
            if (ModelState.IsValid)
            {
                db.Facturaciones.Add(facturacion);
                db.SaveChanges();
                return RedirectToAction("Index"); // Redirige a la página principal o lista de facturaciones
            }

            // Si el modelo no es válido, vuelve a cargar los datos necesarios.
            ViewBag.Id_Paquete = new SelectList(db.Paquetes, "Id", "Nombre", facturacion.Id_Paquete);
            ViewBag.Id_Usuario = new SelectList(db.Usuarios, "Id", "Nombre", facturacion.Id_Usuario);
            return View(facturacion);
        }


    }
}