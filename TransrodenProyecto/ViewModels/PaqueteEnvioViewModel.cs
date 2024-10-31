using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransrodenProyecto.Models;

namespace TransrodenProyecto.ViewModels
{
    public class PaqueteEnvioViewModel
    {
        public List<Paquete> Paquetes { get; set; }

        public List<Envio> Envios { get; set; }
    }
}