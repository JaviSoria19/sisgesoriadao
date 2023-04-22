using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class Cotizacion
    {
        public int IdCotizacion { get; set; }
        public byte IdUsuario { get; set; }
        public byte IdSucursal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public Cotizacion()
        {

        }
        public Cotizacion(int idCotizacion, byte idUsuario, byte idSucursal, DateTime fechaRegistro)
        {
            IdCotizacion = idCotizacion;
            IdUsuario = idUsuario;
            IdSucursal = idSucursal;
            FechaRegistro = fechaRegistro;
        }
        public Cotizacion(byte idUsuario, byte idSucursal)
        {
            IdUsuario = idUsuario;
            IdSucursal = idSucursal;
        }
        public Cotizacion(int idCotizacion)
        {
            IdCotizacion = idCotizacion;
        }
    }
}
