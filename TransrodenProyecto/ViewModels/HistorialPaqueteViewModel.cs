using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransrodenProyecto.Models;

namespace TransrodenProyecto.ViewModels
{
    public class HistorialPaqueteViewModel
    {

        public Paquete Paquete { get; set; }

        public List<Historial> Historial { get; set; } = new List<Historial>();


    }
}