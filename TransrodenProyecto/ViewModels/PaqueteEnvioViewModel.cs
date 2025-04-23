using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransrodenProyecto.Models;

namespace TransrodenProyecto.ViewModels
{


    public class PaqueteEnvioViewModel
    {
        public List<Paquete> Paquetes { get; set; } = new List<Paquete>();
        public List<Envio> Envios { get; set; } = new List<Envio>();

        // Para la paginacion de paquetes
        public int PaquetePageNumber { get; set; } = 1;
        public int PaqueteTotalPages { get; set; } = 1;
        public int PaqueteTotalCount { get; set; }

        // Para la paginacion de envio
        public int EnvioPageNumber { get; set; } = 1;
        public int EnvioTotalPages { get; set; } = 1;
        public int EnvioTotalCount { get; set; }

        public int PageSize { get; set; } = 10;
    }


}