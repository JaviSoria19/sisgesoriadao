using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class Usuario:BaseClass
    {
        public byte IdUsuario { get; set; }
        public byte IdEmpleado { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasenha { get; set; }
        public byte Rol { get; set; }
        public string Pin { get; set; }

        public Usuario()
        {

        }

        public Usuario(byte idUsuario, byte idEmpleado, string nombreUsuario, string contrasenha, byte rol, string pin, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            :base(estado, fechaRegistro, fechaActualizacion)
        {
            IdUsuario = idUsuario;
            IdEmpleado = idEmpleado;
            NombreUsuario = nombreUsuario;
            Contrasenha = contrasenha;
            Rol = rol;
            Pin = pin;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="nombreUsuario"></param>
        /// <param name="contrasenha"></param>
        /// <param name="rol"></param>
        /// <param name="pin"></param>
        public Usuario(byte idEmpleado, string nombreUsuario, string contrasenha, byte rol, string pin)
        {
            IdEmpleado = idEmpleado;
            NombreUsuario = nombreUsuario;
            Contrasenha = contrasenha;
            Rol = rol;
            Pin = pin;
        }
        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="idUsuario"></param>
        public Usuario(byte idUsuario)
        {
            IdUsuario = idUsuario;
        }

        public Usuario(byte idUsuario, string nombreUsuario, byte rol) : this(idUsuario)
        {
            NombreUsuario = nombreUsuario;
            Rol = rol;
        }
    }
}
