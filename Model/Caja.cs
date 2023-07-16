using System;

namespace sisgesoriadao.Model
{
    public class Caja : BaseClass
    {
        public int IdCaja { get; set; }
        public byte IdSucursal { get; set; }
        public byte IdUsuario { get; set; }
        public byte IdUsuarioReceptor { get; set; }
        public Caja()
        {

        }
        /// <summary>
        /// GET
        /// </summary>
        /// <param name="idCaja"></param>
        /// <param name="idSucursal"></param>
        /// <param name="idUsuario"></param>
        /// <param name="idUsuarioReceptor"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Caja(int idCaja, byte idSucursal, byte idUsuario, byte idUsuarioReceptor, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdCaja = idCaja;
            IdSucursal = idSucursal;
            IdUsuario = idUsuario;
            IdUsuarioReceptor = idUsuarioReceptor;
        }
    }
}
