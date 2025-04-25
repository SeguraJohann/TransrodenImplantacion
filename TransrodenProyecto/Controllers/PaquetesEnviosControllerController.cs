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


        public ActionResult DomicilioSJCarga(int? paquetePage, int? envioPage, int? pageSize)
        {
            int currentPageSize = pageSize ?? 10;

            var viewModel = new PaqueteEnvioViewModel
            {
                PageSize = currentPageSize,

                // Paquetes
                Paquetes = db.Paquetes
                    .Where(p => (p.Origen == OrigenPaquete.PerezZeledon && p.Estado == EstadoPaquete.BodegaSJ && p.Domicilio == true && p.Carga.Estado == EstadoCarga.Recibido)
                             || (p.Origen == OrigenPaquete.PerezZeledon && p.Estado == EstadoPaquete.Reenvio && p.Domicilio == true && p.Carga.Estado == EstadoCarga.Recibido))
                    .OrderBy(p => p.Id_Paquete)
                    .Skip(((paquetePage ?? 1) - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList(),

                PaquetePageNumber = paquetePage ?? 1,
                PaqueteTotalCount = db.Paquetes
                    .Count(p => (p.Origen == OrigenPaquete.PerezZeledon && p.Estado == EstadoPaquete.BodegaSJ && p.Domicilio == true && p.Carga.Estado == EstadoCarga.Recibido)
                             || (p.Origen == OrigenPaquete.PerezZeledon && p.Estado == EstadoPaquete.Reenvio && p.Domicilio == true && p.Carga.Estado == EstadoCarga.Recibido)),

                // Envíos
                Envios = db.Envios
                    .Include(c => c.Usuario)
                    .Where(c => c.Estado == EstadoEnvio.BodegaSJ)
                    .OrderBy(c => c.Id_Envio)
                    .Skip(((envioPage ?? 1) - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList(),

                EnvioPageNumber = envioPage ?? 1,
                EnvioTotalCount = db.Envios.Count(c => c.Estado == EstadoEnvio.BodegaSJ)
            };

            viewModel.PaqueteTotalPages = (int)Math.Ceiling((double)viewModel.PaqueteTotalCount / currentPageSize);
            viewModel.EnvioTotalPages = (int)Math.Ceiling((double)viewModel.EnvioTotalCount / currentPageSize);

            return View(viewModel);
        }



        public ActionResult DomicilioPZCarga(int? paquetePage, int? envioPage, int? pageSize)
        {
            int currentPageSize = pageSize ?? 10;

            var viewModel = new PaqueteEnvioViewModel
            {
                PageSize = currentPageSize,

                // Paquetes 
                Paquetes = db.Paquetes
                    .Where(p => (p.Origen == OrigenPaquete.SanJose && p.Estado == EstadoPaquete.BodegaPZ && p.Domicilio == true && p.Carga.Estado == EstadoCarga.Recibido)
                             || (p.Origen == OrigenPaquete.SanJose && p.Estado == EstadoPaquete.Reenvio && p.Domicilio == true && p.Carga.Estado == EstadoCarga.Recibido))
                    .OrderBy(p => p.Id_Paquete)
                    .Skip(((paquetePage ?? 1) - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList(),

                PaquetePageNumber = paquetePage ?? 1,
                PaqueteTotalCount = db.Paquetes
                    .Count(p => (p.Origen == OrigenPaquete.SanJose && p.Estado == EstadoPaquete.BodegaPZ && p.Domicilio == true && p.Carga.Estado == EstadoCarga.Recibido)
                             || (p.Origen == OrigenPaquete.SanJose && p.Estado == EstadoPaquete.Reenvio && p.Domicilio == true && p.Carga.Estado == EstadoCarga.Recibido)),

                // Envíos
                Envios = db.Envios
                    .Include(c => c.Usuario)
                    .Where(c => c.Estado == EstadoEnvio.BodegaPZ)
                    .OrderBy(c => c.Id_Envio)
                    .Skip(((envioPage ?? 1) - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList(),

                EnvioPageNumber = envioPage ?? 1,
                EnvioTotalCount = db.Envios.Count(c => c.Estado == EstadoEnvio.BodegaPZ)
            };

            viewModel.PaqueteTotalPages = (int)Math.Ceiling((double)viewModel.PaqueteTotalCount / currentPageSize);
            viewModel.EnvioTotalPages = (int)Math.Ceiling((double)viewModel.EnvioTotalCount / currentPageSize);

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



        //No se necesita historial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarPaqueteAEnvioSJ(List<PaqueteEnvioAsignacionViewModel> paqueteEnvioAsignaciones)
        {
            if (paqueteEnvioAsignaciones == null || !paqueteEnvioAsignaciones.Any())
            {
                TempData["ErrorMessage"] = "No se han seleccionado cargas o paquetes.";
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
                    var paquete = db.Paquetes.FirstOrDefault(p => p.Id_Paquete == asignacion.IdPaquete && p.Estado == EstadoPaquete.BodegaSJ || p.Id_Paquete == asignacion.IdPaquete && p.Estado == EstadoPaquete.Reenvio);
                    if (paquete == null)
                    {
                        continue;
                    }


                    paquete.Id_Envio = envio.Id_Envio;
                    paquete.Estado = EstadoPaquete.Asignado;

                    envio.NumeroPaquetes = (envio.NumeroPaquetes ?? 0) + 1;
                    TempData["SuccessMessage"] = "Los paquetes se han asignado correctamente.";
                }
            }


            db.SaveChanges();
            return RedirectToAction("DomicilioSJCarga");
        }



        //No se necesita historial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarPaqueteAEnvioPZ(List<PaqueteEnvioAsignacionViewModel> paqueteEnvioAsignaciones)
        {
            if (paqueteEnvioAsignaciones == null || !paqueteEnvioAsignaciones.Any())
            {
                TempData["ErrorMessage"] = "No se han seleccionado cargas o paquetes.";
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
                    var paquete = db.Paquetes.FirstOrDefault(p => p.Id_Paquete == asignacion.IdPaquete && p.Estado == EstadoPaquete.BodegaPZ || p.Id_Paquete == asignacion.IdPaquete && p.Estado == EstadoPaquete.Reenvio);
                    if (paquete == null)
                    {
                        continue;
                    }


                    paquete.Id_Envio = envio.Id_Envio;
                    paquete.Estado = EstadoPaquete.Asignado;

                    envio.NumeroPaquetes = (envio.NumeroPaquetes ?? 0) + 1;
                    TempData["SuccessMessage"] = "Los paquetes se han asignado correctamente.";
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


        [HttpGet]
        public ActionResult EnvioPaquetesTransport(int idEnvio, string searchTracking = "", string searchCedula = "", EstadoPaquete? searchEstado = null, int page = 1)
        {
            // Verificar y obtener el envío
            var envio = db.Envios.Include(c => c.Usuario).FirstOrDefault(c => c.Id_Envio == idEnvio);
            if (envio == null)
            {
                return HttpNotFound("Envío no encontrado");
            }

            int pageSize = 10;

            IQueryable<Paquete> query = db.Paquetes.Where(p => p.Id_Envio == idEnvio);

            if (!string.IsNullOrEmpty(searchTracking))
            {
                query = query.Where(p => p.NumeroRastreo.Contains(searchTracking));
            }

            if (!string.IsNullOrEmpty(searchCedula))
            {
                query = query.Where(p => p.CedulaReceptor.Contains(searchCedula));
            }

            if (searchEstado.HasValue)
            {
                query = query.Where(p => p.Estado == searchEstado.Value);
            }

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var paquetes = query.OrderBy(p => p.NumeroRastreo)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            var estadosFiltrados = new List<EstadoPaquete>
                {
                    EstadoPaquete.Domicilio,
                    EstadoPaquete.Entregado,
                    EstadoPaquete.NoEntregado
                };

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.SearchTracking = searchTracking;
            ViewBag.SearchCedula = searchCedula;
            ViewBag.SearchEstado = searchEstado;
            ViewBag.EstadosPaquete = estadosFiltrados;
            ViewBag.IdEnvio = idEnvio;

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
                TempData["ErrorMessageCarga"] = "Selecciona un estado valido";
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
                        paquete.Estado = EstadoPaquete.Domicilio; // Enum de transito domicilio para la vista de rastreo
                    }
                    else if (envio.Estado == EstadoEnvio.Entregado)
                    {
                        paquete.Estado = EstadoPaquete.Entregado;
                    }

                    var nuevoRastreo = new Historial
                    {
                        Id_Paquete = paquete.Id_Paquete,
                        Estado = paquete.Estado,
                        NumeroRastreo = paquete.NumeroRastreo,
                        Fecha = DateTime.Now
                    };

                    db.Historiales.Add(nuevoRastreo);

                }

                db.SaveChanges();
                TempData["SuccessMessageCarga"] = "Los paquetes se han asignado correctamente.";
            }
            else
            {
                TempData["ErrorMessageCarga"] = "Estado invalido";
            }

            return RedirectToAction("DomicilioSJCarga");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoEnvioPZ(int idEnvio, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                TempData["ErrorMessageCarga"] = "Selecciona un estado valido";
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
                        paquete.Estado = EstadoPaquete.Domicilio;
                    }
                    else if (envio.Estado == EstadoEnvio.Entregado)
                    {
                        paquete.Estado = EstadoPaquete.Entregado;
                    }

                    var nuevoRastreo = new Historial
                    {
                        Id_Paquete = paquete.Id_Paquete,
                        Estado = paquete.Estado,
                        NumeroRastreo = paquete.NumeroRastreo,
                        Fecha = DateTime.Now
                    };

                    db.Historiales.Add(nuevoRastreo);

                }

                db.SaveChanges();
                TempData["SuccessMessageCarga"] = "Los paquetes se han asignado correctamente.";
            }
            else
            {
                TempData["ErrorMessageCarga"] = "Estado invalido";
            }

            return RedirectToAction("DomicilioPZCarga");
        }


        // No esta en funcion de momento

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

                    var nuevoRastreo = new Historial
                    {
                        Id_Paquete = paquete.Id_Paquete,
                        Estado = paquete.Estado,
                        NumeroRastreo = paquete.NumeroRastreo,
                        Fecha = DateTime.Now
                    };

                    db.Historiales.Add(nuevoRastreo);

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
                //ModelState.AddModelError("", "Seleccione un Estado!!");
                TempData["ErrorMessageEnvio"] = "No se han seleccionado un estado";
                return Redirect(Request.UrlReferrer.ToString());
            }

            var envio = db.Envios.Include(c => c.Paquetes).FirstOrDefault(c => c.Id_Envio == idEnvio);

            if (envio == null)
            {
                return HttpNotFound("Carga no encontrada!");
            }

            // Se valida que el envio no tenga paquetes y si los tiene que estos esten Entregados
            //if (envio.Paquetes == null || !envio.Paquetes.Any() || envio.Paquetes.All(p => p.Estado == EstadoPaquete.Entregado) || envio.Paquetes.All(p => p.Estado == EstadoPaquete.NoEntregado))
            //if (envio.Paquetes == null || !envio.Paquetes.Any() || envio.Paquetes.Any(p => p.Estado != EstadoPaquete.EnTransito))
            //if (envio.Paquetes == null || !envio.Paquetes.Any() || envio.Paquetes.Any(p => p.Estado == EstadoPaquete.Entregado) || envio.Paquetes.Any(p => p.Estado == EstadoPaquete.NoEntregado))
            if ((envio.Paquetes == null || !envio.Paquetes.Any() || envio.Paquetes.Any(p => p.Estado == EstadoPaquete.Entregado) || envio.Paquetes.Any(p => p.Estado == EstadoPaquete.NoEntregado)) && !envio.Paquetes.Any(p => p.Estado == EstadoPaquete.Domicilio) && !envio.Paquetes.Any(p => p.Estado == EstadoPaquete.EnTransito))
            {

                if (Enum.TryParse<EstadoEnvio>(nuevoEstado, out var estadoResult))
                {
                    envio.Estado = estadoResult;
                    TempData["SuccessMessageEnvio"] = "El Envio ha sido completado!";
                    db.SaveChanges();
                }
                else
                {
                    //ModelState.AddModelError("", "Estado inválido!!");
                    TempData["ErrorMessageEnvio"] = "No se han seleccionado un estado";
                }
            }
            else
            {
                // Mostrar mensaje de error si hay paquetes pendientes de entrega
                //ModelState.AddModelError("", "Existen paquetes pendientes de entrega para completar la entrega.");
                TempData["ErrorMessageEnvioEstado"] = "Aun existen paquetes pendiente de entrega!!";
            }

            return Redirect(Request.UrlReferrer.ToString());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoPaquete(int idPaquete, string nuevoEstado, string cedulaReceptor = null)
        {
            if (string.IsNullOrEmpty(nuevoEstado))
            {
                TempData["Error"] = "Seleccione un estado válido.";
                return Redirect(Request.UrlReferrer.ToString());
            }

            var paquete = db.Paquetes.Find(idPaquete);
            if (paquete == null)
            {
                TempData["Error"] = "Paquete no encontrado.";
                return Redirect(Request.UrlReferrer.ToString());
            }

            var estadoEnum = (EstadoPaquete)Enum.Parse(typeof(EstadoPaquete), nuevoEstado);

            if (estadoEnum == EstadoPaquete.Entregado)
            {
                if (string.IsNullOrEmpty(cedulaReceptor) || !cedulaReceptor.Equals(paquete.CedulaReceptor, StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Error"] = "La cédula ingresada no coincide con la registrada para este paquete.";
                    return Redirect(Request.UrlReferrer.ToString());
                }

                paquete.fecha_entrega = DateTime.Now;
            }

            paquete.Estado = estadoEnum;

            var nuevoRastreo = new Historial
            {
                Id_Paquete = paquete.Id_Paquete,
                Estado = paquete.Estado,
                NumeroRastreo = paquete.NumeroRastreo,
                Fecha = DateTime.Now
            };

            db.Historiales.Add(nuevoRastreo);
            db.SaveChanges();

            TempData["Success"] = "El estado del paquete se ha actualizado correctamente.";
            return Redirect(Request.UrlReferrer.ToString());
        }

        /******************************************************************* EDITANDO**********************************************************************************************/

        // Metodo para quitar los paquetes de una carga
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuitarPaquete(int idPaquete, int idEnvio)
        {

            //Se busca primero el paquete
            var paquete = db.Paquetes.FirstOrDefault(p => p.Id_Paquete == idPaquete && p.Id_Envio == idEnvio);
            var envio = db.Envios.FirstOrDefault(c => c.Id_Envio.Equals(idEnvio));


            if (paquete == null)
            {
                return HttpNotFound("El paquete no fue encontrado o no pertenece a la carga especificada.");
            }


            // Aqui vuelve a pasar a nulo (estado original del campo)
            paquete.Id_Envio = null;


            // Poner el estado original del paquete
            if (paquete.Origen == OrigenPaquete.SanJose)
            {
                paquete.Estado = EstadoPaquete.BodegaPZ;
            }
            else if (paquete.Origen == OrigenPaquete.PerezZeledon)
            {
                paquete.Estado = EstadoPaquete.BodegaSJ;
            }
            else
            {
                paquete.Estado = EstadoPaquete.SinAsignar;
            }

            envio.NumeroPaquetes = (envio.NumeroPaquetes ?? 0) - 1;

            db.SaveChanges();

            return RedirectToAction("EnvioPaquetes", new { idEnvio = idEnvio });
        }






        // Paquete rechazado que necesita ser reenviado

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReenvioPZ(int idPaquete, int idEnvio)
        {

            //Se busca primero el paquete
            var paquete = db.Paquetes.FirstOrDefault(p => p.Id_Paquete == idPaquete && p.Id_Envio == idEnvio);
            var envio = db.Envios.FirstOrDefault(c => c.Id_Envio.Equals(idEnvio));


            if (paquete == null)
            {
                return HttpNotFound("El paquete no fue encontrado o no pertenece a la carga especificada.");
            }


            // Aqui vuelve a pasar a nulo (estado original del campo)
            paquete.Id_Envio = null;


            // Poner el estado original del paquete
            if (paquete.Origen == OrigenPaquete.SanJose)
            {
                paquete.Estado = EstadoPaquete.Reenvio;
            }
            else if (paquete.Origen == OrigenPaquete.PerezZeledon)
            {
                paquete.Estado = EstadoPaquete.Reenvio;
            }
            else
            {
                paquete.Estado = EstadoPaquete.SinAsignar;
            }

            //Se guarda en el historial
            var nuevoRastreo = new Historial
            {
                Id_Paquete = paquete.Id_Paquete,
                Estado = paquete.Estado,
                NumeroRastreo = paquete.NumeroRastreo,
                Fecha = DateTime.Now
            };

            db.Historiales.Add(nuevoRastreo);


            db.SaveChanges();

            return RedirectToAction("PaquetesRechazoPZ", "PaquetesCargasController");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReenvioSJ(int idPaquete, int idEnvio)
        {

            //Se busca primero el paquete
            var paquete = db.Paquetes.FirstOrDefault(p => p.Id_Paquete == idPaquete && p.Id_Envio == idEnvio);
            var envio = db.Envios.FirstOrDefault(c => c.Id_Envio.Equals(idEnvio));


            if (paquete == null)
            {
                return HttpNotFound("El paquete no fue encontrado o no pertenece a la carga especificada.");
            }


            // Aqui vuelve a pasar a nulo (estado original del campo)
            paquete.Id_Envio = null;


            // Poner el estado original del paquete
            if (paquete.Origen == OrigenPaquete.SanJose)
            {
                paquete.Estado = EstadoPaquete.Reenvio;
            }
            else if (paquete.Origen == OrigenPaquete.PerezZeledon)
            {
                paquete.Estado = EstadoPaquete.Reenvio;
            }
            else
            {
                paquete.Estado = EstadoPaquete.SinAsignar;
            }

            //Se guarda en el historial
            var nuevoRastreo = new Historial
            {
                Id_Paquete = paquete.Id_Paquete,
                Estado = paquete.Estado,
                NumeroRastreo = paquete.NumeroRastreo,
                Fecha = DateTime.Now
            };

            db.Historiales.Add(nuevoRastreo);


            db.SaveChanges();

            return RedirectToAction("PaquetesRechazoSJ", "PaquetesCargasController");
        }







        // ++++++++++++++++++++++++++++++++++++++++++ Vista transportista del modulo de tracking ++++++++++++++++++++++++++++++++++


        //Para ver las cargas de envios asignadas al transportista

        public ActionResult EnviosTransportista(int? searchId, EstadoEnvio? searchEstado, int page = 1)
        {
            // Verificar sesión
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            int pageSize = 10;
            var usuarioId = (int)Session["UsuarioId"];
            var usuarioRol = (Rol)Session["UsuarioRol"];


            IQueryable<Envio> query = db.Envios.Include(e => e.Usuario)
                .Where(e => e.Id_Usuario == usuarioId &&
                          (e.Estado == EstadoEnvio.EnTransito ||
                           e.Estado == EstadoEnvio.BodegaSJ ||
                           e.Estado == EstadoEnvio.BodegaPZ));

            if (searchId.HasValue)
            {
                query = query.Where(e => e.Id_Envio == searchId.Value);
            }

            if (searchEstado.HasValue)
            {
                query = query.Where(e => e.Estado == searchEstado.Value);
            }

            // Paginación
            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var envios = query.OrderBy(e => e.Id_Envio)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.SearchId = searchId;
            ViewBag.SearchEstado = searchEstado;
            ViewBag.EstadosEnvio = Enum.GetValues(typeof(EstadoEnvio))
                                      .Cast<EstadoEnvio>()
                                      .Where(e => e == EstadoEnvio.EnTransito ||
                                                e == EstadoEnvio.BodegaSJ ||
                                                e == EstadoEnvio.BodegaPZ)
                                      .ToList();

            var viewModel = new PaqueteEnvioViewModel
            {
                Envios = envios
            };

            return View(viewModel);
        }




    }
}


