using System;

namespace sisgesoriadao.Model
{
    public class Condicion : BaseClass
    {
        public byte IdCondicion { get; set; }
        public byte IdUsuario { get; set; }
        public string NombreCondicion { get; set; }
        public Condicion()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idCondicion"></param>
        /// <param name="idUsuario"></param>
        /// <param name="nombreCondicion"></param>
        public Condicion(byte idCondicion, byte idUsuario, string nombreCondicion, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdCondicion = idCondicion;
            IdUsuario = idUsuario;
            NombreCondicion = nombreCondicion;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="nombreCondicion"></param>
        public Condicion(byte idUsuario, string nombreCondicion)
        {
            IdUsuario = idUsuario;
            NombreCondicion = nombreCondicion;
        }
        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="idCondicion"></param>
        public Condicion(byte idCondicion)
        {
            IdCondicion = idCondicion;
        }
    }
}
