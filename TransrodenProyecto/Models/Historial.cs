using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransrodenProyecto.Models
{
    public class Historial
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Historial { get; set; }


        [Required]
        public int Id_Paquete { get; set; }
        public Paquete Paquete { get; set; }


        [StringLength(50)]
        public string NumeroRastreo { get; set; } = string.Empty;


        //Enum
        public EstadoPaquete Estado { get; set; }


        [Required]
        public DateTime Fecha { get; set; }


    }
}