using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TransrodenProyecto.Models;
using TransrodenProyecto.ViewModels;
using TransrodenProyecto.Security;


namespace TransrodenProyecto.Controllers
{
    public class PaquetesCargasControllerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // ++++++++++++++++++++++++++++++++++++++++++ Vista del modulo de tracking ++++++++++++++++++++++++++++++++++

        //Menu para todo relacionado a Tracking SJ
        [AdminBodOnly]
        public ActionResult DashboardSJ()
        {
            return View();
        }


        //Menu para todo relacionado a Tracking PZ
        [AdminBodOnly]
        public ActionResult DashboardPZ()
        {
            return View();
        }


        //Menu para la bodega de SJ
        [AdminBodOnly]
        public ActionResult BodegaSJ()
        {
            return View();
        }

        [AdminBodOnly]
        //Menu para la bodega de PZ
        public ActionResult BodegaPZ()
        {
            return View();
        }

        [AdminBodOnly]
        public ActionResult AsignarPaqueteSJCarga(
            int? paquetePage,
            int? cargaPage,
            int? cargaRecibidaPage,
            int? pageSize)
        {
            int currentPageSize = pageSize ?? 10;

            var viewModel = new PaqueteCargaViewModel
            {
                PageSize = currentPageSize,

                // Paquetes con paginación
                Paquetes = db.Paquetes
                    .Where(p => p.Estado == EstadoPaquete.SinAsignarSJ && p.Origen == OrigenPaquete.SanJose)
                    .OrderBy(p => p.Id_Paquete)
                    .Skip(((paquetePage ?? 1) - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList(),

                PaquetePageNumber = paquetePage ?? 1,
                PaqueteTotalCount = db.Paquetes.Count(p => p.Estado == EstadoPaquete.SinAsignarSJ && p.Origen == OrigenPaquete.SanJose),

                // Cargas con paginación
                Cargas = db.Cargas
                    .Include(c => c.Usuario)
                    .Where(c => c.Estado == EstadoCarga.BodegaSJ && c.Origen == OrigenCarga.SanJose)
                    .OrderBy(c => c.Id_Carga)
                    .Skip(((cargaPage ?? 1) - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList(),

                CargaPageNumber = cargaPage ?? 1,
                CargaTotalCount = db.Cargas.Count(c => c.Estado == EstadoCarga.BodegaSJ && c.Origen == OrigenCarga.SanJose),

                // Cargas recibidas con paginación
                CargasRecibidas = db.Cargas
                    .Include(c => c.Usuario)
                    .Where(c => c.Estado == EstadoCarga.BodegaSJ && c.Origen == OrigenCarga.PerezZeledon)
                    .OrderBy(c => c.Id_Carga)
                    .Skip(((cargaRecibidaPage ?? 1) - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList(),

                CargaRecibidaPageNumber = cargaRecibidaPage ?? 1,
                CargaRecibidaTotalCount = db.Cargas.Count(c => c.Estado == EstadoCarga.BodegaSJ && c.Origen == OrigenCarga.PerezZeledon)
            };

            // Calcular total de páginas
            viewModel.PaqueteTotalPages = (int)Math.Ceiling((double)viewModel.PaqueteTotalCount / currentPageSize);
            viewModel.CargaTotalPages = (int)Math.Ceiling((double)viewModel.CargaTotalCount / currentPageSize);
            viewModel.CargaRecibidaTotalPages = (int)Math.Ceiling((double)viewModel.CargaRecibidaTotalCount / currentPageSize);

            return View(viewModel);
        }






        [AdminBodOnly]
        public ActionResult AsignarPaquetePZCarga(
            int? paquetePage,
            int? cargaPage,
            int? cargaRecibidaPage,
            int? pageSize)
        {
            // Configurar tamaño de página (con valor por defecto 10)
            int currentPageSize = pageSize ?? 10;

            var viewModel = new PaqueteCargaViewModel
            {
                PageSize = currentPageSize,

                // Paquetes con paginación
                Paquetes = db.Paquetes
                    .Where(p => p.Estado == EstadoPaquete.SinAsignarPZ && p.Origen == OrigenPaquete.PerezZeledon)
                    .OrderBy(p => p.Id_Paquete)
                    .Skip(((paquetePage ?? 1) - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList(),

                PaquetePageNumber = paquetePage ?? 1,
                PaqueteTotalCount = db.Paquetes.Count(p => p.Estado == EstadoPaquete.SinAsignarPZ && p.Origen == OrigenPaquete.PerezZeledon),

                // Cargas con paginación
                Cargas = db.Cargas
                    .Include(c => c.Usuario)
                    .Where(c => c.Estado == EstadoCarga.BodegaPZ && c.Origen == OrigenCarga.PerezZeledon)
                    .OrderBy(c => c.Id_Carga)
                    .Skip(((cargaPage ?? 1) - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList(),

                CargaPageNumber = cargaPage ?? 1,
                CargaTotalCount = db.Cargas.Count(c => c.Estado == EstadoCarga.BodegaPZ && c.Origen == OrigenCarga.PerezZeledon),

                // Cargas recibidas con paginación
                CargasRecibidas = db.Cargas
                    .Include(c => c.Usuario)
                    .Where(c => c.Estado == EstadoCarga.BodegaPZ && c.Origen == OrigenCarga.SanJose)
                    .OrderBy(c => c.Id_Carga)
                    .Skip(((cargaRecibidaPage ?? 1) - 1) * currentPageSize)
                    .Take(currentPageSize)
                    .ToList(),

                CargaRecibidaPageNumber = cargaRecibidaPage ?? 1,
                CargaRecibidaTotalCount = db.Cargas.Count(c => c.Estado == EstadoCarga.BodegaPZ && c.Origen == OrigenCarga.SanJose)
            };

            // Calcular total de páginas para cada sección
            viewModel.PaqueteTotalPages = (int)Math.Ceiling((double)viewModel.PaqueteTotalCount / currentPageSize);
            viewModel.CargaTotalPages = (int)Math.Ceiling((double)viewModel.CargaTotalCount / currentPageSize);
            viewModel.CargaRecibidaTotalPages = (int)Math.Ceiling((double)viewModel.CargaRecibidaTotalCount / currentPageSize);

            return View(viewModel);
        }




        [AdminBodOnly]
        //Ver cargas en transito
        public ActionResult VistaCargaTransito()
        {
            var viewModel = new PaqueteCargaViewModel
            {
                Cargas = db.Cargas.Include(c => c.Usuario).Where(c => c.Estado == EstadoCarga.EnTransito || c.Estado == EstadoCarga.Averia).ToList(),
                Envios = db.Envios.Include(c => c.Usuario).Where(c => c.Estado == EstadoEnvio.EnTransito).ToList()
            };

            return View(viewModel);
        }


        [AdminBodOnly]
        //Todos los paquetes que se encuentra en la bodega SJ
        public ActionResult PaquetesBodegaSJ(string searchTerm = null, string filterBy = null, int page = 1)
        {
            // Tamaño de la paginacion
            int pageSize = 10;

            var query = db.Paquetes.Where(p =>
                p.Estado == EstadoPaquete.SinAsignarSJ && p.Origen == OrigenPaquete.SanJose ||
                p.Estado == EstadoPaquete.BodegaSJ && p.Origen == OrigenPaquete.SanJose ||
                p.Estado == EstadoPaquete.BodegaSJ && p.Origen == OrigenPaquete.PerezZeledon && p.Carga.Estado == EstadoCarga.Recibido ||
                p.Estado == EstadoPaquete.NoEntregado && p.Origen == OrigenPaquete.PerezZeledon && p.Carga.Estado == EstadoCarga.Recibido && p.Envio.Estado == EstadoEnvio.Entregado);

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy)
                {
                    case "NumeroRastreo":
                        query = query.Where(p => p.NumeroRastreo.Contains(searchTerm));
                        break;
                    case "CedulaEmisor":
                        query = query.Where(p => p.CedulaEmisor.Contains(searchTerm));
                        break;
                    case "Tipo":
                        if (Enum.TryParse(searchTerm, out TipoPaquete tipo))
                        {
                            query = query.Where(p => p.Tipo == tipo);
                        }
                        break;
                }
            }


            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var paquetes = query.OrderBy(p => p.NumeroRastreo)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.FilterBy = filterBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            var viewModel = new PaqueteCargaViewModel
            {
                Paquetes = paquetes
            };

            return View(viewModel);
        }





        [AdminBodOnly]
        //Todos los paquetes que se encuentra en la bodega SJ que no son domicilio
        public ActionResult PaquetesReclamoSJ(string searchTerm = null, string filterBy = null, int page = 1)
        {
            int pageSize = 10;


            var query = db.Paquetes
                .Where(p => p.Estado == EstadoPaquete.BodegaSJ
                         && p.Origen == OrigenPaquete.PerezZeledon
                         && p.Domicilio == false
                         && p.Carga.Estado == EstadoCarga.Recibido);

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy)
                {
                    case "NumeroRastreo":
                        query = query.Where(p => p.NumeroRastreo.Contains(searchTerm));
                        break;
                    case "NombreEmisor":
                        query = query.Where(p => p.NombreEmisor.Contains(searchTerm));
                        break;
                    case "NombreReceptor":
                        query = query.Where(p => p.NombreReceptor.Contains(searchTerm));
                        break;
                    case "CedulaReceptor":
                        query = query.Where(p => p.CedulaReceptor.Contains(searchTerm));
                        break;
                }
            }

            // Calcular el total de registros y paginas
            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var paquetes = query.OrderBy(p => p.NumeroRastreo)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.FilterBy = filterBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            var viewModel = new PaqueteCargaViewModel
            {
                Paquetes = paquetes
            };

            return View(viewModel);
        }








        [AdminBodOnly]
        //Todos los paquetes que se encuentra en la bodega PZ

        public ActionResult PaquetesBodegaPZ(string searchTerm = null, string filterBy = null, int page = 1)
        {
            // Tamaño de la paginacion
            int pageSize = 10;

            var query = db.Paquetes.Where(p =>
                p.Estado == EstadoPaquete.SinAsignarPZ && p.Origen == OrigenPaquete.PerezZeledon ||
                p.Estado == EstadoPaquete.BodegaPZ && p.Origen == OrigenPaquete.PerezZeledon ||
                p.Estado == EstadoPaquete.BodegaPZ && p.Origen == OrigenPaquete.SanJose && p.Carga.Estado == EstadoCarga.Recibido ||
                p.Estado == EstadoPaquete.NoEntregado && p.Origen == OrigenPaquete.SanJose && p.Carga.Estado == EstadoCarga.Recibido && p.Envio.Estado == EstadoEnvio.Entregado);

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy)
                {
                    case "NumeroRastreo":
                        query = query.Where(p => p.NumeroRastreo.Contains(searchTerm));
                        break;
                    case "CedulaEmisor":
                        query = query.Where(p => p.CedulaEmisor.Contains(searchTerm));
                        break;
                    case "Tipo":
                        if (Enum.TryParse(searchTerm, out TipoPaquete tipo))
                        {
                            query = query.Where(p => p.Tipo == tipo);
                        }
                        break;
                }
            }


            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var paquetes = query.OrderBy(p => p.NumeroRastreo)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.FilterBy = filterBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            var viewModel = new PaqueteCargaViewModel
            {
                Paquetes = paquetes
            };

            return View(viewModel);
        }









        [AdminBodOnly]
        //Todos los paquetes que se encuentra en la bodega SJ que no son domicilio

        public ActionResult PaquetesReclamoPZ(string searchTerm = null, string filterBy = null, int page = 1)
        {
            // Tamaño de página configurable
            int pageSize = 10;


            var query = db.Paquetes
                .Where(p => p.Estado == EstadoPaquete.BodegaPZ 
                         && p.Origen == OrigenPaquete.SanJose 
                         && p.Domicilio == false 
                         && p.Carga.Estado == EstadoCarga.Recibido);

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy)
                {
                    case "NumeroRastreo":
                        query = query.Where(p => p.NumeroRastreo.Contains(searchTerm));
                        break;
                    case "NombreEmisor":
                        query = query.Where(p => p.NombreEmisor.Contains(searchTerm));
                        break;
                    case "NombreReceptor":
                        query = query.Where(p => p.NombreReceptor.Contains(searchTerm));
                        break;
                    case "CedulaReceptor":
                        query = query.Where(p => p.CedulaReceptor.Contains(searchTerm));
                        break;
                }
            }

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var paquetes = query.OrderBy(p => p.NumeroRastreo)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.FilterBy = filterBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            var viewModel = new PaqueteCargaViewModel
            {
                Paquetes = paquetes
            };

            return View(viewModel);
        }






        [AdminBodOnly]
        public ActionResult PaquetesRechazoSJ(string searchTerm = null, string filterBy = null, int page = 1)
        {
            // Tamaño de página configurable
            int pageSize = 10;

            // Consulta base
            var query = db.Paquetes.Include(p => p.Envio)
                .Where(p => p.Estado == EstadoPaquete.NoEntregado && p.Origen == OrigenPaquete.PerezZeledon && p.Carga.Estado == EstadoCarga.Recibido && p.Envio.Estado == EstadoEnvio.Entregado);

            // Aplicar filtros si existen
            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy)
                {
                    case "NumeroRastreo":
                        query = query.Where(p => p.NumeroRastreo.Contains(searchTerm));
                        break;
                    case "NombreEmisor":
                        query = query.Where(p => p.NombreEmisor.Contains(searchTerm));
                        break;
                    case "NombreReceptor":
                        query = query.Where(p => p.NombreReceptor.Contains(searchTerm));
                        break;
                    case "CedulaReceptor":
                        query = query.Where(p => p.CedulaReceptor.Contains(searchTerm));
                        break;
                }
            }

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var paquetes = query.OrderBy(p => p.NumeroRastreo)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.FilterBy = filterBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            var viewModel = new PaqueteCargaViewModel
            {
                Paquetes = paquetes
            };

            return View(viewModel);
        }







        [AdminBodOnly]
        public ActionResult PaquetesRechazoPZ(string searchTerm = null, string filterBy = null, int page = 1)
        {
            // Tamaño de paginacion
            int pageSize = 10;

            var query = db.Paquetes.Include(p => p.Envio)
                .Where(p => p.Estado == EstadoPaquete.NoEntregado
                         && p.Origen == OrigenPaquete.SanJose
                         && p.Carga.Estado == EstadoCarga.Recibido
                         && p.Envio.Estado == EstadoEnvio.Entregado);

            if (!string.IsNullOrEmpty(searchTerm) && !string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy)
                {
                    case "NumeroRastreo":
                        query = query.Where(p => p.NumeroRastreo.Contains(searchTerm));
                        break;
                    case "NombreEmisor":
                        query = query.Where(p => p.NombreEmisor.Contains(searchTerm));
                        break;
                    case "NombreReceptor":
                        query = query.Where(p => p.NombreReceptor.Contains(searchTerm));
                        break;
                    case "CedulaReceptor":
                        query = query.Where(p => p.CedulaReceptor.Contains(searchTerm));
                        break;
                }
            }


            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);


            var paquetes = query.OrderBy(p => p.NumeroRastreo)
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.FilterBy = filterBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;

            var viewModel = new PaqueteCargaViewModel
            {
                Paquetes = paquetes
            };

            return View(viewModel);
        }







        // ++++++++++++++++++++++++++++++++++++++++++ Metodo del modulo de tracking ++++++++++++++++++++++++++++++++++


        // Metodo para asignar los paquetes a las cargas SJ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarPaqueteACargaSJ(List<PaqueteCargaAsignacionViewModel> paqueteCargaAsignaciones)
        {
            if (paqueteCargaAsignaciones == null || !paqueteCargaAsignaciones.Any())
            {
                TempData["ErrorMessage"] = "No se han seleccionado cargas o paquetes.";
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

                    // Obtiene el paquete que se va asignar
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
            //TempData["SuccessMessage"] = "Los paquetes se han asignado correctamente.";
            return RedirectToAction("AsignarPaqueteSJCarga");
        }



        // Metodo para asignar los paquetes a las cargas PZ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarPaqueteACargaPZ(List<PaqueteCargaAsignacionViewModel> paqueteCargaAsignaciones)
        {
            if (paqueteCargaAsignaciones == null || !paqueteCargaAsignaciones.Any())
            {
                TempData["ErrorMessage"] = "No se han seleccionado cargas o paquetes.";
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
            //TempData["SuccessMessage"] = "Los paquetes se han asignado correctamente.";
            return RedirectToAction("AsignarPaquetePZCarga");
        }



        // Metodo para quitar los paquetes de una carga
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuitarPaquete(int idPaquete, int idCarga)
        {

            //Se busca primero el paquete
            var paquete = db.Paquetes.FirstOrDefault(p => p.Id_Paquete == idPaquete && p.Id_Carga == idCarga);
            var carga = db.Cargas.FirstOrDefault(c => c.Id_Carga.Equals(idCarga));


            if (paquete == null)
            {
                return HttpNotFound("El paquete no fue encontrado o no pertenece a la carga especificada.");
            }


            // Aqui vuelve a pasar a nulo (estado original del campo)
            paquete.Id_Carga = null;


            // Poner el estado original del paquete
            if (paquete.Origen == OrigenPaquete.SanJose)
            {
                paquete.Estado = EstadoPaquete.SinAsignarSJ;
            }
            else if (paquete.Origen == OrigenPaquete.PerezZeledon)
            {
                paquete.Estado = EstadoPaquete.SinAsignarPZ;
            }
            else
            {
                paquete.Estado = EstadoPaquete.SinAsignar;
            }

            carga.NumeroPaquetes = (carga.NumeroPaquetes ?? 0) - 1;

            db.SaveChanges();

            return RedirectToAction("CargaPaquetes", new { idCarga = idCarga });
        }




        [AdminBodOnly]
        // Muestra los paquetes que estan asignados a la carga pero para las vistas de asignacion
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

        [AdminBodOnly]
        //Para otras vista donde solo se requiera ver el paquete nada mas
        public ActionResult CargaPaquetesView(int idCarga)
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


        // Vista para Transportistas donde solo se requiera ver el paquete nada mas

        public ActionResult CargaPaquetesViewTransp(int idCarga)
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






        // Actualizar el estado de la carga la cual tambien cambiara la de los paquetes SJ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoCargaSJ(int idCarga, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                TempData["ErrorMessageCarga"] = "No se han seleccionado un estado";
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


                    var nuevoRastreo = new Historial
                    {
                        Id_Paquete = paquete.Id_Paquete,
                        Estado = paquete.Estado,
                        NumeroRastreo = paquete.NumeroRastreo, 
                        Fecha = DateTime.Now
                    };

                    db.Historiales.Add(nuevoRastreo);

                }
                TempData["SuccessMessageCarga"] = "La carga se encuentra en Transito!";
                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Estado invalido!!");
            }

            return RedirectToAction("AsignarPaqueteSJCarga");
        }



        // Actualizar el estado de la carga la cual tambien cambiara la de los paquetes PZ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoCargaPZ(int idCarga, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                TempData["ErrorMessageCarga"] = "No se han seleccionado un estado";
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

                    var nuevoRastreo = new Historial
                    {
                        Id_Paquete = paquete.Id_Paquete,
                        Estado = paquete.Estado,
                        NumeroRastreo = paquete.NumeroRastreo,
                        Fecha = DateTime.Now
                    };

                    db.Historiales.Add(nuevoRastreo);

                }
                TempData["SuccessMessageCarga"] = "La carga se encuentra en Transito!";
                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Estado invalido!!");
            }

            return RedirectToAction("AsignarPaquetePZCarga");
        }







        //Cambia el estado solo de la carga, esto es para las cargas que vienen de PZ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstadoCargaSJ(int idCarga, string nuevoEstado)
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

                if (carga.Estado == EstadoCarga.Recibido)
                {
                    carga.fecha_entrega = DateTime.Now;
                    TempData["SuccessMessageReceptor"] = "La carga ha sido aceptada";
                }
                
                db.SaveChanges();
            }
            else
            {
                TempData["ErrorMessageReceptor"] = "La carga no fue aceptada";
            }

            return RedirectToAction("AsignarPaqueteSJCarga");
        }



        //Cambia el estado solo de la carga, esto es para las cargas que vienen de SJ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstadoCargaPZ(int idCarga, string nuevoEstado)
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

                if (carga.Estado == EstadoCarga.Recibido)
                {
                    carga.fecha_entrega = DateTime.Now;
                    TempData["SuccessMessageReceptor"] = "La carga ha sido aceptada";
                }
                
                db.SaveChanges();
            }
            else
            {
                TempData["ErrorMessageReceptor"] = "La carga no fue aceptada";
            }

            return RedirectToAction("AsignarPaquetePZCarga");
        }
 



        // SOlO PARA TRANSPORTISTA Y SOLO PARA REPORTE DE AVERIA // NO AFECTA PAQUETES Y NO CAMBIA EL ESTADO EN HISTORIAL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EstadoCargaTransp(int idCarga, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                ModelState.AddModelError("", "Seleccione un Estado!!");
                return Redirect("CargasTransportista");
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

                db.SaveChanges();
            }
            else
            {
                ModelState.AddModelError("", "Estado invalido!!");
            }

            return Redirect("CargasTransportista");
        }




        // Cambiar el estado de las cargas en la vista de cargas en transito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoCargaGlobal(int idCarga, string nuevoEstado)
        {

            if (string.IsNullOrEmpty(nuevoEstado))
            {
                TempData["ErrorMessageGlobalCarga"] = "No se ha seleccionado un estado!! ";
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

                    var nuevoRastreo = new Historial
                    {
                        Id_Paquete = paquete.Id_Paquete,
                        Estado = paquete.Estado,
                        NumeroRastreo = paquete.NumeroRastreo,
                        Fecha = DateTime.Now
                    };

                    db.Historiales.Add(nuevoRastreo);

                }

                if (carga.Estado == EstadoCarga.BodegaPZ)
                {
                    TempData["SuccessMessageGlobalCarga"] = "La carga se encuentra en la Bodega Perez Zeledon";
                }
                else if (carga.Estado == EstadoCarga.BodegaSJ)
                {
                    TempData["SuccessMessageGlobalCarga"] = "La carga se encuentra en la Bodega San Jose";
                }
                
                db.SaveChanges();
            }
            else
            {
                TempData["ErrorMessageGlobalCarga"] = "No se ha seleccionado un estado!! ";
                ModelState.AddModelError("", "Estado invalido!!");
            }

            return RedirectToAction("VistaCargaTransito");
        }








        // Cambia solo el estado del paquete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstadoPaquete(int idPaquete, string nuevoEstado)
        {
            if (string.IsNullOrEmpty(nuevoEstado))
            {
                TempData["Error"] = "Seleccione un estado válido.";
                return RedirectToAction("PaquetesBodegaSJ");
            }

            var paquete = db.Paquetes.Find(idPaquete);
            if (paquete == null)
            {
                TempData["Error"] = "Paquete no encontrado!!";
                return RedirectToAction("PaquetesBodegaSJ");
            }


            paquete.Estado = (EstadoPaquete)Enum.Parse(typeof(EstadoPaquete), nuevoEstado);
            
            if (paquete.Estado == EstadoPaquete.Entregado)
            {
                paquete.fecha_entrega = DateTime.Now;
            }


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
            //return RedirectToAction("PaquetesBodegaSJ");
            //Redirige a la vista desde donde fue accionado el metodo
            return Redirect(Request.UrlReferrer.ToString()); // ---------------------------------------------- < CAMBIAR > ----------------------------
        }



        // ++++++++++++++++++++++++++++++++++++++++++ Vista transportista del modulo de tracking ++++++++++++++++++++++++++++++++++


        // Muesta todos las cargas que tiene el usuario Transportista asignado
        public ActionResult CargasTransportista(int? searchId, EstadoCarga? searchEstado, int page = 1)
        {
            // Verificar sesión
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            int pageSize = 10;
            var usuarioId = (int)Session["UsuarioId"];
            var usuarioRol = (Rol)Session["UsuarioRol"];


            IQueryable<Carga> query = db.Cargas.Include(c => c.Usuario)
                .Where(c => c.Id_Usuario == usuarioId &&
                          (c.Estado == EstadoCarga.EnTransito ||
                           c.Estado == EstadoCarga.BodegaSJ ||
                           c.Estado == EstadoCarga.BodegaPZ ||
                           c.Estado == EstadoCarga.Averia));


            if (searchId.HasValue)
            {
                query = query.Where(c => c.Id_Carga == searchId.Value);
            }

            if (searchEstado.HasValue)
            {
                query = query.Where(c => c.Estado == searchEstado.Value);
            }

            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var cargas = query.OrderByDescending(c => c.fecha_creacion)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.SearchId = searchId;
            ViewBag.SearchEstado = searchEstado;
            ViewBag.EstadosCarga = Enum.GetValues(typeof(EstadoCarga))
                                      .Cast<EstadoCarga>()
                                      .Where(e => e == EstadoCarga.EnTransito ||
                                                e == EstadoCarga.BodegaSJ ||
                                                e == EstadoCarga.BodegaPZ ||
                                                e == EstadoCarga.Averia)
                                      .ToList();

            var viewModel = new PaqueteCargaViewModel
            {
                Cargas = cargas
            };

            return View(viewModel);
        }









        // Muesta todos las cargas entregadas que tiene el usuario Transportista asignado
        public ActionResult CargasEntregadasTransportista(int? searchId, DateTime? searchDate, int page = 1)
        {
            // Verificar sesión
            if (Session["UsuarioId"] == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            int pageSize = 10;
            var usuarioId = (int)Session["UsuarioId"];
            var usuarioRol = (Rol)Session["UsuarioRol"];

            IQueryable<Carga> query = db.Cargas.Include(c => c.Usuario)
                .Where(c => c.Id_Usuario == usuarioId &&
                          (c.Estado == EstadoCarga.Entregado ||
                           c.Estado == EstadoCarga.Recibido));


            if (searchId.HasValue)
            {
                query = query.Where(c => c.Id_Carga == searchId.Value);
            }

            if (searchDate.HasValue)
            {
                var fechaInicio = searchDate.Value.Date;
                var fechaFin = fechaInicio.AddDays(1);

                query = query.Where(c => c.fecha_creacion >= fechaInicio &&
                                        c.fecha_creacion < fechaFin);
            }


            int totalRecords = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var cargas = query.OrderByDescending(c => c.fecha_creacion)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();


            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.SearchId = searchId;
            ViewBag.SearchDate = searchDate?.ToString("yyyy-MM-dd");

            var viewModel = new PaqueteCargaViewModel
            {
                Cargas = cargas
            };

            return View(viewModel);
        }





    }
}
