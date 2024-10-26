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
    public class PaquetesCargasControllerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult AsignarPaqueteSJCarga()
        {
            var viewModel = new PaqueteCargaViewModel
            {
                Paquetes = db.Paquetes.Where(p => p.Estado == EstadoPaquete.SinAsignarSJ).ToList(),
                Cargas = db.Cargas.Include(c => c.Usuario).Where(c => c.Estado == EstadoCarga.BodegaSJ).ToList()
            };

            return View(viewModel);
        }


        public ActionResult AsignarPaquetePZCarga()
        {
            var viewModel = new PaqueteCargaViewModel
            {
                Paquetes = db.Paquetes.Where(p => p.Estado == EstadoPaquete.SinAsignarPZ).ToList(),
                Cargas = db.Cargas.Include(c => c.Usuario).Where(c => c.Estado == EstadoCarga.BodegaPZ).ToList()
            };

            return View(viewModel);
        }


        public ActionResult VistaCargaTransito()
        {
            var viewModel = new PaqueteCargaViewModel
            {
                Cargas = db.Cargas.Include(c => c.Usuario).Where(c => c.Estado == EstadoCarga.EnTransito).ToList()
            };

            return View(viewModel);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarPaqueteACargaSJ(List<PaqueteCargaAsignacionViewModel> paqueteCargaAsignaciones)
        {
            if (paqueteCargaAsignaciones == null || !paqueteCargaAsignaciones.Any())
            {
                ModelState.AddModelError("", "No se han seleccionado cargas o paquetes");
                return RedirectToAction("AsignarPaqueteSJCarga");
            }

            foreach (var asignacion in paqueteCargaAsignaciones)
            {
                
                if (asignacion.IdCarga > 0)
                {
                    // Verifica si la carga existe
                    var carga = db.Cargas.FirstOrDefault(c => c.Id_Carga == asignacion.IdCarga && c.Estado == EstadoCarga.BodegaSJ);
                    if (carga == null)
                    {
                        continue; // Continuar si la carga no es valida
                    }

                    // Obtenie el paquete que se va asignar
                    var paquete = db.Paquetes.FirstOrDefault(p => p.Id_Paquete == asignacion.IdPaquete && p.Estado == EstadoPaquete.SinAsignarSJ);
                    if (paquete == null)
                    {
                        continue; 
                    }


                    paquete.Id_Carga = carga.Id_Carga;
                    paquete.Estado = EstadoPaquete.BodegaSJ;

                    carga.NumeroPaquetes = (carga.NumeroPaquetes ?? 0) + 1;
                }
            }


            db.SaveChanges();
            return RedirectToAction("AsignarPaqueteSJCarga");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarPaqueteACargaPZ(List<PaqueteCargaAsignacionViewModel> paqueteCargaAsignaciones)
        {
            if (paqueteCargaAsignaciones == null || !paqueteCargaAsignaciones.Any())
            {
                ModelState.AddModelError("", "No se han seleccionado cargas o paquetes");
                return RedirectToAction("AsignarPaquetePZCarga");
            }

            foreach (var asignacion in paqueteCargaAsignaciones)
            {

                if (asignacion.IdCarga > 0)
                {

                    var carga = db.Cargas.FirstOrDefault(c => c.Id_Carga == asignacion.IdCarga && c.Estado == EstadoCarga.BodegaPZ);
                    if (carga == null)
                    {
                        continue; 
                    }

                    var paquete = db.Paquetes.FirstOrDefault(p => p.Id_Paquete == asignacion.IdPaquete && p.Estado == EstadoPaquete.SinAsignarPZ);
                    if (paquete == null)
                    {
                        continue;
                    }

                    paquete.Id_Carga = carga.Id_Carga;
                    paquete.Estado = EstadoPaquete.BodegaPZ;

                    carga.NumeroPaquetes = (carga.NumeroPaquetes ?? 0) + 1;
                }
            }


            db.SaveChanges();
            return RedirectToAction("AsignarPaquetePZCarga");
        }




        public ActionResult CargaPaquetes(int idCarga)
        {

            
            var carga = db.Cargas.Include(c => c.Usuario).FirstOrDefault(c => c.Id_Carga == idCarga);

            if (carga == null)
            {
                return HttpNotFound("Cargas no encontradas");
            }

            // Paquetes asociados a la carga
            var paquetes = db.Paquetes.Where(p => p.Id_Carga == idCarga).ToList();


            // Este viewModel funciona para cargar la vista
            var viewModel = new PaqueteCargaViewModel
            {
                Cargas = new List<Carga> { carga },
                Paquetes = paquetes
            };

            return View(viewModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoCargaSJ(int idCarga, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                ModelState.AddModelError("", "Seleccione un Estado!!");
                return RedirectToAction("AsignarPaqueteSJCarga");
            }


            var carga = db.Cargas.Include(c => c.Paquetes).FirstOrDefault(c => c.Id_Carga == idCarga);

            if (carga == null)
            {
                return HttpNotFound("Carga no encontrada!");
            }



            // TryParse convierte un string en un valor enum
            if (Enum.TryParse<EstadoCarga>(nuevoEstado, out var estadoResult))
            {
                carga.Estado = estadoResult;


                foreach (var paquete in carga.Paquetes)
                {
                    if (carga.Estado == EstadoCarga.BodegaSJ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaSJ;
                    }
                    else if (carga.Estado == EstadoCarga.BodegaPZ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaPZ;
                    }
                    else if (carga.Estado == EstadoCarga.EnTransito)
                    {
                        paquete.Estado = EstadoPaquete.EnTransito;
                    }
                    else if (carga.Estado == EstadoCarga.Entregado)
                    {
                        paquete.Estado = EstadoPaquete.Entregado;
                    }
                }

                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Estado invalido!!");
            }

            return RedirectToAction("AsignarPaqueteSJCarga");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoCargaPZ(int idCarga, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                ModelState.AddModelError("", "Seleccione un Estado!!");
                return RedirectToAction("AsignarPaquetePZCarga");
            }


            var carga = db.Cargas.Include(c => c.Paquetes).FirstOrDefault(c => c.Id_Carga == idCarga);

            if (carga == null)
            {
                return HttpNotFound("Carga no encontrada!");
            }



            // TryParse convierte un string en un valor enum
            if (Enum.TryParse<EstadoCarga>(nuevoEstado, out var estadoResult))
            {
                carga.Estado = estadoResult;


                foreach (var paquete in carga.Paquetes)
                {
                    if (carga.Estado == EstadoCarga.BodegaSJ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaSJ;
                    }
                    else if (carga.Estado == EstadoCarga.BodegaPZ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaPZ;
                    }
                    else if (carga.Estado == EstadoCarga.EnTransito)
                    {
                        paquete.Estado = EstadoPaquete.EnTransito;
                    }
                    else if (carga.Estado == EstadoCarga.Entregado)
                    {
                        paquete.Estado = EstadoPaquete.Entregado;
                    }
                }

                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Estado invalido!!");
            }

            return RedirectToAction("AsignarPaquetePZCarga");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoCargaGlobal(int idCarga, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                ModelState.AddModelError("", "Seleccione un Estado!!");
                return RedirectToAction("VistaCargaTransito");
            }


            var carga = db.Cargas.Include(c => c.Paquetes).FirstOrDefault(c => c.Id_Carga == idCarga);

            if (carga == null)
            {
                return HttpNotFound("Carga no encontrada!");
            }



            // TryParse convierte un string en un valor enum
            if (Enum.TryParse<EstadoCarga>(nuevoEstado, out var estadoResult))
            {
                carga.Estado = estadoResult;


                foreach (var paquete in carga.Paquetes)
                {
                    if (carga.Estado == EstadoCarga.BodegaSJ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaSJ;
                    }
                    else if (carga.Estado == EstadoCarga.BodegaPZ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaPZ;
                    }
                    else if (carga.Estado == EstadoCarga.EnTransito)
                    {
                        paquete.Estado = EstadoPaquete.EnTransito;
                    }
                    else if (carga.Estado == EstadoCarga.Entregado)
                    {
                        paquete.Estado = EstadoPaquete.Entregado;
                    }
                }

                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Estado invalido!!");
            }

            return RedirectToAction("VistaCargaTransito");
        }



    }
}
