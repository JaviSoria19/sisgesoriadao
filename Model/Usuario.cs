using System;

namespace sisgesoriadao.Model
{
    public class Usuario : BaseClass
    {
        public byte IdUsuario { get; set; }
        public byte IdEmpleado { get; set; }
        public byte IdAjustes { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasenha { get; set; }
        public byte Rol { get; set; }
        public string Pin { get; set; }

        public Usuario()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="idEmpleado"></param>
        /// <param name="idAjustes"></param>
        /// <param name="nombreUsuario"></param>
        /// <param name="contrasenha"></param>
        /// <param name="rol"></param>
        /// <param name="pin"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Usuario(byte idUsuario, byte idEmpleado, byte idAjustes, string nombreUsuario, string contrasenha, byte rol, string pin, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdUsuario = idUsuario;
            IdEmpleado = idEmpleado;
            IdAjustes = idAjustes;
            NombreUsuario = nombreUsuario;
            Contrasenha = contrasenha;
            Rol = rol;
            Pin = pin;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="idAjustes"></param>
        /// <param name="nombreUsuario"></param>
        /// <param name="contrasenha"></param>
        /// <param name="rol"></param>
        /// <param name="pin"></param>
        public Usuario(byte idEmpleado, byte idAjustes, string nombreUsuario, string contrasenha, byte rol, string pin)
        {
            IdEmpleado = idEmpleado;
            IdAjustes = idAjustes;
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
