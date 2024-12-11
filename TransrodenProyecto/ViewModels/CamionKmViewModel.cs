using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransrodenProyecto.Models;
using System.Web.Mvc;

namespace TransrodenProyecto.ViewModels
{
    public class CamionKmViewModel
    {
        public IEnumerable<Kilometraje> Kilometros { get; set; }
        public Camion CamionActual { get; set; }
        public IEnumerable<SelectListItem> CamionesDisponibles { get; set; }
    }
}