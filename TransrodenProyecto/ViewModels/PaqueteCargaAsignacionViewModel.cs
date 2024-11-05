using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransrodenProyecto.ViewModels
{

    // Se tuvo que crear para relacionar el paquete y la carga
    public class PaqueteCargaAsignacionViewModel
    {
        public int IdPaquete { get; set; }
        public int IdCarga { get; set; }
    }
}