using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class Sucursal : BaseClass
    {
        public byte IdSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
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
        public Sucursal(byte idSucursal, string nombreSucursal, string direccion, string correo, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdSucursal = idSucursal;
            NombreSucursal = nombreSucursal;
            Direccion = direccion;
            Correo = correo;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="nombreSucursal"></param>
        /// <param name="direccion"></param>
        /// <param name="correo"></param>
        public Sucursal(string nombreSucursal, string direccion, string correo)
        {
            NombreSucursal = nombreSucursal;
            Direccion = direccion;
            Correo = correo;
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
