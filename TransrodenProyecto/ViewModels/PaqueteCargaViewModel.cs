using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransrodenProyecto.Models;

namespace TransrodenProyecto.ViewModels
{


    public class PaqueteCargaViewModel
    {
        // Propiedades existentes
        //public List<Paquete> Paquetes { get; set; }
        //public List<Paquete> PaquetesRecibidos { get; set; }
        //public List<Carga> Cargas { get; set; }
        //public List<Carga> CargasRecibidas { get; set; }
        //public List<Envio> Envios { get; set; }

        public List<Paquete> Paquetes { get; set; } = new List<Paquete>();
        public List<Paquete> PaquetesRecibidos { get; set; } = new List<Paquete>();
        public List<Carga> Cargas { get; set; } = new List<Carga>();
        public List<Carga> CargasRecibidas { get; set; } = new List<Carga>();
        public List<Envio> Envios { get; set; } = new List<Envio>();


        // Nuevas propiedades para paginación de Paquetes Sin Asignar
        public int PaquetePageNumber { get; set; } = 1;
        public int PaqueteTotalPages { get; set; } = 1;
        public int PaqueteTotalCount { get; set; }

        // Paginación para Cargas
        public int CargaPageNumber { get; set; } = 1;
        public int CargaTotalPages { get; set; } = 1;
        public int CargaTotalCount { get; set; }

        // Paginación para Cargas Recibidas
        public int CargaRecibidaPageNumber { get; set; } = 1;
        public int CargaRecibidaTotalPages { get; set; } = 1;
        public int CargaRecibidaTotalCount { get; set; }

        // Tamaño de página común (puedes hacerlo configurable por sección si lo necesitas)
        public int PageSize { get; set; } = 10;

        // Propiedades para mantener los filtros actuales (opcional)
        public string CurrentFilterPaquetes { get; set; }
        public string CurrentFilterCargas { get; set; }
        public string CurrentFilterCargasRecibidas { get; set; }


    }






    //public class PaqueteCargaViewModel
    //{
    //    public List<Paquete> Paquetes { get; set; }
    //    public List<Paquete> PaquetesRecibidos { get; set; }

    //    public List<Carga> Cargas { get; set; }
    //    public List<Carga> CargasRecibidas { get; set; }

    //    public List<Envio> Envios { get; set; }
    //}
}