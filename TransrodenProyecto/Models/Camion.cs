using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransrodenProyecto.Models
{
    public class Camion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Camion { get; set; }


        // Para que acepte null
        public int? Id_Usuario { get; set; }
        public Usuario Usuario { get; set; }
        

        [Required]
        public string Marca { get; set; }


        [Required]
        public string Modelo { get; set; }


        //Enum
        [Required]
        public TipoCarga TipoCarga { get; set; }



        // Que no esta averiado
        [Required]
        public bool Disponible { get; set; }
    }
}