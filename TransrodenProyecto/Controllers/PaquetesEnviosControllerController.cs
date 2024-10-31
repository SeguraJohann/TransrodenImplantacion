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
    public class PaquetesEnviosControllerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult DomicilioSJCarga()
        {
            var viewModel = new PaqueteEnvioViewModel
            {
                Paquetes = db.Paquetes.Where(p => p.Origen == OrigenPaquete.PerezZeledon && p.Estado == EstadoPaquete.BodegaSJ && p.Domicilio == true).ToList(),
                Envios = db.Envios.Include(c => c.Usuario).Where(c => c.Estado == EstadoEnvio.BodegaSJ).ToList()

            };

            return View(viewModel);
        }


        public ActionResult DomicilioPZCarga()
        {
            var viewModel = new PaqueteEnvioViewModel
            {
                Paquetes = db.Paquetes.Where(p => p.Origen == OrigenPaquete.SanJose && p.Estado == EstadoPaquete.BodegaPZ && p.Domicilio == true).ToList(),
                Envios = db.Envios.Include(c => c.Usuario).Where(c => c.Estado == EstadoEnvio.BodegaPZ).ToList()

            };

            return View(viewModel);
        }

        public ActionResult VistaEnvioTransito()
        {
            var viewModel = new PaqueteEnvioViewModel
            {
                Envios = db.Envios.Include(c => c.Usuario).Where(c => c.Estado == EstadoEnvio.EnTransito).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarPaqueteAEnvioSJ(List<PaqueteEnvioAsignacionViewModel> paqueteEnvioAsignaciones)
        {
            if (paqueteEnvioAsignaciones == null || !paqueteEnvioAsignaciones.Any())
            {
                ModelState.AddModelError("", "No se han seleccionado envios o paquetes");
                return RedirectToAction("DomicilioSJCarga");
            }

            foreach (var asignacion in paqueteEnvioAsignaciones)
            {

                if (asignacion.IdEnvio > 0)
                {
                    // Verifica si la carga existe
                    var envio = db.Envios.FirstOrDefault(c => c.Id_Envio == asignacion.IdEnvio && c.Estado == EstadoEnvio.BodegaSJ);
                    if (envio == null)
                    {
                        continue; // Continuar si la carga no es valida
                    }

                    // Obtenie el paquete que se va asignar
                    var paquete = db.Paquetes.FirstOrDefault(p => p.Id_Paquete == asignacion.IdPaquete && p.Estado == EstadoPaquete.BodegaSJ);
                    if (paquete == null)
                    {
                        continue;
                    }


                    paquete.Id_Envio = envio.Id_Envio;
                    paquete.Estado = EstadoPaquete.Asignado;

                    envio.NumeroPaquetes = (envio.NumeroPaquetes ?? 0) + 1;
                }
            }


            db.SaveChanges();
            return RedirectToAction("DomicilioSJCarga");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarPaqueteAEnvioPZ(List<PaqueteEnvioAsignacionViewModel> paqueteEnvioAsignaciones)
        {
            if (paqueteEnvioAsignaciones == null || !paqueteEnvioAsignaciones.Any())
            {
                ModelState.AddModelError("", "No se han seleccionado envios o paquetes");
                return RedirectToAction("DomicilioPZCarga");
            }

            foreach (var asignacion in paqueteEnvioAsignaciones)
            {

                if (asignacion.IdEnvio > 0)
                {
                    // Verifica si la carga existe
                    var envio = db.Envios.FirstOrDefault(c => c.Id_Envio == asignacion.IdEnvio && c.Estado == EstadoEnvio.BodegaPZ);
                    if (envio == null)
                    {
                        continue; // Continuar si la carga no es valida
                    }

                    // Obtenie el paquete que se va asignar
                    var paquete = db.Paquetes.FirstOrDefault(p => p.Id_Paquete == asignacion.IdPaquete && p.Estado == EstadoPaquete.BodegaPZ);
                    if (paquete == null)
                    {
                        continue;
                    }


                    paquete.Id_Envio = envio.Id_Envio;
                    paquete.Estado = EstadoPaquete.Asignado;

                    envio.NumeroPaquetes = (envio.NumeroPaquetes ?? 0) + 1;
                }
            }


            db.SaveChanges();
            return RedirectToAction("DomicilioPZCarga");
        }


        // Para Bodeguero
        public ActionResult EnvioPaquetes(int idEnvio)
        {


            var envio = db.Envios.Include(c => c.Usuario).FirstOrDefault(c => c.Id_Envio == idEnvio);

            if (envio == null)
            {
                return HttpNotFound("Envios no encontradas");
            }

            // Paquetes asociados a la carga
            var paquetes = db.Paquetes.Where(p => p.Id_Envio == idEnvio).ToList();


            // Este viewModel funciona para cargar la vista
            var viewModel = new PaqueteEnvioViewModel
            {
                Envios = new List<Envio> { envio },
                Paquetes = paquetes
            };

            return View(viewModel);
        }



        // Para Transportista

        public ActionResult EnvioPaquetesTransport(int idEnvio)
        {


            var envio = db.Envios.Include(c => c.Usuario).FirstOrDefault(c => c.Id_Envio == idEnvio);

            if (envio == null)
            {
                return HttpNotFound("Envios no encontradas");
            }

            // Paquetes asociados a la carga
            var paquetes = db.Paquetes.Where(p => p.Id_Envio == idEnvio).ToList();


            // Este viewModel funciona para cargar la vista
            var viewModel = new PaqueteEnvioViewModel
            {
                Envios = new List<Envio> { envio },
                Paquetes = paquetes
            };

            return View(viewModel);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoEnvioSJ(int idEnvio, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                ModelState.AddModelError("", "Seleccione un Estado!!");
                return RedirectToAction("DomicilioSJCarga");
            }


            var envio = db.Envios.Include(c => c.Paquetes).FirstOrDefault(c => c.Id_Envio == idEnvio);

            if (envio == null)
            {
                return HttpNotFound("Envio no encontrada!");
            }



            // TryParse convierte un string en un valor enum
            if (Enum.TryParse<EstadoEnvio>(nuevoEstado, out var estadoResult))
            {
                envio.Estado = estadoResult;


                foreach (var paquete in envio.Paquetes)
                {
                    if (envio.Estado == EstadoEnvio.BodegaSJ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaSJ;
                    }
                    else if (envio.Estado == EstadoEnvio.BodegaPZ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaPZ;
                    }
                    else if (envio.Estado == EstadoEnvio.EnTransito)
                    {
                        paquete.Estado = EstadoPaquete.EnTransito;
                    }
                    else if (envio.Estado == EstadoEnvio.Entregado)
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

            return RedirectToAction("DomicilioSJCarga");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoEnvioPZ(int idEnvio, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                ModelState.AddModelError("", "Seleccione un Estado!!");
                return RedirectToAction("DomicilioPZCarga");
            }


            var envio = db.Envios.Include(c => c.Paquetes).FirstOrDefault(c => c.Id_Envio == idEnvio);

            if (envio == null)
            {
                return HttpNotFound("Envio no encontrada!");
            }



            // TryParse convierte un string en un valor enum
            if (Enum.TryParse<EstadoEnvio>(nuevoEstado, out var estadoResult))
            {
                envio.Estado = estadoResult;


                foreach (var paquete in envio.Paquetes)
                {
                    if (envio.Estado == EstadoEnvio.BodegaSJ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaSJ;
                    }
                    else if (envio.Estado == EstadoEnvio.BodegaPZ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaPZ;
                    }
                    else if (envio.Estado == EstadoEnvio.EnTransito)
                    {
                        paquete.Estado = EstadoPaquete.EnTransito;
                    }
                    else if (envio.Estado == EstadoEnvio.Entregado)
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

            return RedirectToAction("DomicilioPZCarga");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoEnvioGlobal(int idEnvio, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                ModelState.AddModelError("", "Seleccione un Estado!!");
                return RedirectToAction("VistaCargaTransito");
            }


            var envio = db.Envios.Include(c => c.Paquetes).FirstOrDefault(c => c.Id_Envio == idEnvio);

            if (envio == null)
            {
                return HttpNotFound("Envio no encontrada!");
            }



            // TryParse convierte un string en un valor enum
            if (Enum.TryParse<EstadoEnvio>(nuevoEstado, out var estadoResult))
            {
                envio.Estado = estadoResult;


                foreach (var paquete in envio.Paquetes)
                {
                    if (envio.Estado == EstadoEnvio.BodegaSJ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaSJ;
                    }
                    else if (envio.Estado == EstadoEnvio.BodegaPZ)
                    {
                        paquete.Estado = EstadoPaquete.BodegaPZ;
                    }
                    else if (envio.Estado == EstadoEnvio.EnTransito)
                    {
                        paquete.Estado = EstadoPaquete.EnTransito;
                    }
                    else if (envio.Estado == EstadoEnvio.Entregado)
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
        // GET: PaquetesEnviosController
        public ActionResult Index()
        {
            return View();
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstadoEnvioTransp(int idEnvio, string nuevoEstado)
        {
            if (string.IsNullOrEmpty(nuevoEstado))
            {
                ModelState.AddModelError("", "Seleccione un Estado!!");
                return Redirect(Request.UrlReferrer.ToString());
            }

            var envio = db.Envios.Include(c => c.Paquetes).FirstOrDefault(c => c.Id_Envio == idEnvio);

            if (envio == null)
            {
                return HttpNotFound("Carga no encontrada!");
            }

            // Se valida que el envio no tenga paquetes y si los tiene que estos esten Entregados
            if (envio.Paquetes == null || !envio.Paquetes.Any() || envio.Paquetes.All(p => p.Estado == EstadoPaquete.Entregado))
            {

                if (Enum.TryParse<EstadoEnvio>(nuevoEstado, out var estadoResult))
                {
                    envio.Estado = estadoResult;
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "Estado inválido!!");
                }
            }
            else
            {
                // Mostrar mensaje de error si hay paquetes pendientes de entrega
                ModelState.AddModelError("", "Existen paquetes pendientes de entrega para completar la entrega.");
            }

            return Redirect(Request.UrlReferrer.ToString());
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoPaquete(int idPaquete, string nuevoEstado)
        {
            if (string.IsNullOrEmpty(nuevoEstado))
            {
                TempData["Error"] = "Seleccione un estado válido.";
                return Redirect(Request.UrlReferrer.ToString());
            }

            var paquete = db.Paquetes.Find(idPaquete);
            if (paquete == null)
            {
                TempData["Error"] = "Paquete no encontrado!!";
                return Redirect(Request.UrlReferrer.ToString());
            }


            paquete.Estado = (EstadoPaquete)Enum.Parse(typeof(EstadoPaquete), nuevoEstado);

            if (paquete.Estado == EstadoPaquete.Entregado)
            {
                paquete.fecha_entrega = DateTime.Now;
            }

            db.SaveChanges();


            TempData["Success"] = "El estado del paquete se ha actualizado correctamente.";
            //return RedirectToAction("PaquetesBodegaSJ");
            //Redirige a la vista desde donde fue accionado el metodo
            return Redirect(Request.UrlReferrer.ToString());
        }




        // ++++++++++++++++++++++++++++++++++++++++++ Vista transportista del modulo de tracking ++++++++++++++++++++++++++++++++++


        //Para ver las cargas de envios asignadas al transportista
        public ActionResult EnviosTransportista()
        {
            // Verificar si la sesión contiene la información del usuario
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            // Obtener el usuario
            var usuarioId = (int)Session["UsuarioId"];
            var usuarioRol = (Rol)Session["UsuarioRol"];


            // Verificar si 'sede' tiene un valor antes de convertirlo
            var envios = new List<Envio>();

            if (usuarioRol == Rol.Transportista)
            {
                //MUESTRA LAS CARGAS QUE SON PERTENECIENTES AL TRANSPORTISTA Y TENGAN ESTADO ENTRANSITO, BODEGASJ, BODEGAPZ
                envios = db.Envios.Include(c => c.Usuario).Where(c => c.Id_Usuario == usuarioId && c.Estado == EstadoEnvio.EnTransito
                    || c.Id_Usuario == usuarioId && c.Estado == EstadoEnvio.BodegaSJ
                    || c.Id_Usuario == usuarioId && c.Estado == EstadoEnvio.BodegaPZ).ToList();
            }

            var viewModel = new PaqueteEnvioViewModel
            {
                Envios = envios
            };

            return View(viewModel);
        }



    }
}


