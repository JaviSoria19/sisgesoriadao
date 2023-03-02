using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class Marca : BaseClass
    {
        public byte IdMarca { get; set; }
        public string NombreMarca { get; set; }
        public byte IdUsuario { get; set; }
        public Marca()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idMarca"></param>
        /// <param name="nombreMarca"></param>
        /// <param name="idUsuario"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Marca(byte idMarca, string nombreMarca, byte idUsuario, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdMarca = idMarca;
            NombreMarca = nombreMarca;
            IdUsuario = idUsuario;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="nombreMarca"></param>
        /// <param name="idUsuario"></param>
        public Marca(string nombreMarca, byte idUsuario)
        {
            NombreMarca = nombreMarca;
            IdUsuario = idUsuario;
        }
        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="idMarca"></param>
        public Marca(byte idMarca)
        {
            IdMarca = idMarca;
        }
    }
}
