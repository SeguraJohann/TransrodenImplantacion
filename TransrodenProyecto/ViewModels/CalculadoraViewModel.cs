using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TransrodenProyecto.ViewModels
{
    public class CalculadoraViewModel
    {
        public int Id_Caja { get; set; }
        public int Id_Entrega { get; set; } 
        public int Cantidad { get; set; } 
        public bool Domicilio { get; set; } 

        //Dropdowns
        public IEnumerable<SelectListItem> TiposCaja { get; set; }
        public IEnumerable<SelectListItem> TiposEntrega { get; set; }

        public decimal CostoTotal { get; set; }
    }
}