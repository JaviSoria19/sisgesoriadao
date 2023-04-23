using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class Caja
    {
        public int IdCaja { get; set; }
        public byte IdSucursal { get; set; }
        public byte IdUsuario { get; set; }
        public byte Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public Caja()
        {

        }
        /// <summary>
        /// GET
        /// </summary>
        /// <param name="idCaja"></param>
        /// <param name="idSucursal"></param>
        /// <param name="idUsuario"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        public Caja(int idCaja, byte idSucursal, byte idUsuario, byte estado, DateTime fechaRegistro)
        {
            IdCaja = idCaja;
            IdSucursal = idSucursal;
            IdUsuario = idUsuario;
            Estado = estado;
            FechaRegistro = fechaRegistro;
        }
        public Caja(int idCaja)
        {
            IdCaja = idCaja;
        }
    }
}
