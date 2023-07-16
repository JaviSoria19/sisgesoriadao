using System;

namespace sisgesoriadao.Model
{
    public class Sucursal : BaseClass
    {
        public byte IdSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public Sucursal()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <param name="nombreSucursal"></param>
        /// <param name="direccion"></param>
        /// <param name="correo"></param>
        /// <param name="telefono"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Sucursal(byte idSucursal, string nombreSucursal, string direccion, string correo, string telefono, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdSucursal = idSucursal;
            NombreSucursal = nombreSucursal;
            Direccion = direccion;
            Correo = correo;
            Telefono = telefono;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="nombreSucursal"></param>
        /// <param name="direccion"></param>
        /// <param name="correo"></param>
        /// <param name="telefono"></param>
        public Sucursal(string nombreSucursal, string direccion, string correo, string telefono)
        {
            NombreSucursal = nombreSucursal;
            Direccion = direccion;
            Correo = correo;
            Telefono = telefono;
        }

        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="idSucursal"></param>
        public Sucursal(byte idSucursal)
        {
            IdSucursal = idSucursal;
        }
    }
}
