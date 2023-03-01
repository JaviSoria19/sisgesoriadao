using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class Categoria : BaseClass
    {
        public byte IdCategoria { get; set; }
        public string NombreCategoria { get; set; }
        public byte IdUsuario { get; set; }
        public Categoria()
        {

        }
        /// <summary>
        /// GET  & UPDATE
        /// </summary>
        /// <param name="idCategoria"></param>
        /// <param name="nombreCategoria"></param>
        /// <param name="idUsuario"></param>
        public Categoria(byte idCategoria, string nombreCategoria, byte idUsuario, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdCategoria = idCategoria;
            NombreCategoria = nombreCategoria;
            IdUsuario = idUsuario;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="nombreCategoria"></param>
        /// <param name="idUsuario"></param>
        public Categoria(string nombreCategoria, byte idUsuario)
        {
            NombreCategoria = nombreCategoria;
            IdUsuario = idUsuario;
        }
        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="idCategoria"></param>
        public Categoria(byte idCategoria)
        {
            IdCategoria = idCategoria;
        }
    }
}
