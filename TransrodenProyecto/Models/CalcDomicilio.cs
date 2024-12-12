using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TransrodenProyecto.Models;

namespace TransrodenProyecto.Calculadora
{
    public class CalcDomicilio
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Domicilio { get; set; }
        public string Nombre { get; set; }
        public decimal Costo { get; set; }

    }
}