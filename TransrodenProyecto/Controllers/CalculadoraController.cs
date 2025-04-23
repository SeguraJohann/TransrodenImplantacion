using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Models;
using TransrodenProyecto.ViewModels;
namespace TransrodenProyecto.Controllers
{
    public class CalculadoraController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Calculadora()
        {

            var tiposCaja = db.CalcCaja.ToList().Select(tc => new SelectListItem{Value = tc.Id_Caja.ToString(), Text = $"{tc.Tipo} (₡{tc.Costo})" }).ToList();

            var tiposEntrega = db.CalcEntrega.ToList().Select(te => new SelectListItem { Value = te.Id_Entrega.ToString(), Text = $"{te.Tipo} (₡{te.Tarifa})"}).ToList();


            var viewModel = new CalculadoraViewModel
            {
                TiposCaja = tiposCaja,
                TiposEntrega = tiposEntrega
            };

            return View(viewModel);
        }



        [HttpPost]
        public ActionResult Calculadora(CalculadoraViewModel model)
        {


            model.TiposCaja = db.CalcCaja.ToList().Select(tc => new SelectListItem { Value = tc.Id_Caja.ToString(), Text = $"{tc.Tipo} (₡{tc.Costo})" }).ToList();

            model.TiposEntrega = db.CalcEntrega.ToList() .Select(te => new SelectListItem { Value = te.Id_Entrega.ToString(), Text = $"{te.Tipo} (₡{te.Tarifa})" }).ToList();


            if (ModelState.IsValid)
            {

                var tipoCaja = db.CalcCaja.Find(model.Id_Caja);

                var tipoEntrega = db.CalcEntrega.Find(model.Id_Entrega);

                if (tipoCaja != null && tipoEntrega != null)
                {
                    var domicilioExtra = model.Domicilio ? db.CalcDomicilios.FirstOrDefault(c => c.Nombre == "DomicilioExtra")?.Costo ?? 0 : 0;

                    model.CostoTotal = (tipoCaja.Costo * model.Cantidad) + tipoEntrega.Tarifa + domicilioExtra;
                }
                else
                {
                    ModelState.AddModelError("", "Error al calcular el costo. Verifique los datos seleccionados.");
                }
            }

            return View(model);
        }



        public ActionResult ObtenerCostoDomicilio()
        {
            var costoDomicilio = db.CalcDomicilios.FirstOrDefault()?.Costo ?? 0;

            return Json(new { costo = costoDomicilio }, JsonRequestBehavior.AllowGet);
        }

    }

}