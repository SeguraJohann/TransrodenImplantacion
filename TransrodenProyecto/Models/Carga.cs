using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransrodenProyecto.Models
{
    public class Carga
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Carga { get; set; }


        // Para que acepte null
        public int Id_Usuario { get; set; }
        public Usuario Usuario { get; set; }


        //Requerido
        public string Descripcion { get; set; }


        //Permite null
        public int? NumeroPaquetes { get; set; }


        //Enum
        public EstadoCarga Estado { get; set; }


        //Enum
        public OrigenCarga Origen { get; set; }



        public DateTime? fecha_creacion { get; set; }

        public DateTime? fecha_entrega { get; set; }


        public List<Paquete> Paquetes { get; set; }
    }
}