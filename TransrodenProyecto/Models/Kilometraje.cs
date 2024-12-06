using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransrodenProyecto.Models
{
    public class Kilometraje
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Km { get; set; }


        [Required]
        public int Id_Camion { get; set; }
        public Camion Camion { get; set; }

        public int Km_anterior { get; set; }

        public int Km_actual { get; set; }

        public string Transportista { get; set; }

        public DateTime Registro { get; set; }

    }
}