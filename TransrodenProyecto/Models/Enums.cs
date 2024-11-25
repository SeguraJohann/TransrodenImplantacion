using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransrodenProyecto.Models
{
    public enum TipoPaquete
    {
        Bodega = 0,
        SanVito = 1,
        Farmacia = 2,
        Repuestos = 3,
        Ruta = 4,
        PalmarNorte = 5

    }

    public enum EstadoPaquete
    {
        SinAsignar = 0,
        SinAsignarSJ = 1,
        SinAsignarPZ = 2,
        Asignado = 3,
        EnTransito = 4,
        BodegaPZ = 5,
        BodegaSJ = 6,
        Domicilio = 7,
        Entregado = 8,
        NoEntregado = 9,
        Reenvio = 10,
    }

    //Para poder quitar un paquete de una carga y ponerlo en su estado original
    public enum OrigenPaquete
    {
        Otro = 0,
        SanJose = 1,
        PerezZeledon = 2   
    }


    public enum EstadoCarga
    {
        Asignado = 0,
        EnTransito = 1,
        BodegaPZ = 2,
        BodegaSJ = 3,
        Recibido = 4,
        Entregado = 5,
        Averia = 6
    }


    //Para colocar la carga en la vista correcta
    public enum OrigenCarga
    {
        Otro = 0,
        SanJose = 1,
        PerezZeledon = 2
    }


    public enum EstadoEnvio
    {
        Asignado = 0,
        EnTransito = 1,
        BodegaPZ = 2,
        BodegaSJ = 3,
        Entregado = 4
    }
    public enum Rol
    {
        Administrador = 0,
        Bodeguero = 1,
        Transportista = 2,
        Cliente = 3
    }

    public enum TipoCliente
    {
        PersonaFisica = 0,
        Empresa = 1
    }

    public enum Sede
    {
        SanJose = 0,
        PerezZeledon = 1
    }


}