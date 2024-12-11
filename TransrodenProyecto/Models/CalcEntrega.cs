using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TransrodenProyecto.Calculadora
{
    public class CalcEntrega
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_Entrega { get; set; }
        public string Tipo { get; set; }
        public decimal Tarifa { get; set; }
    }
}