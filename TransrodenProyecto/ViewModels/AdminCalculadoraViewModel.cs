using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransrodenProyecto.Calculadora;
using TransrodenProyecto.Models;

namespace TransrodenProyecto.ViewModels
{
    public class AdminCalculadoraViewModel
    {
        public List<CalcCaja> Caja { get; set; }

        public List<CalcDomicilio> Domicilio { get; set; }

        public List<CalcEntrega> Entrega { get; set; }
    }
}