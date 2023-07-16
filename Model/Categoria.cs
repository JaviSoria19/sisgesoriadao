using System;

namespace sisgesoriadao.Model
{
    public class Categoria : BaseClass
    {
        public byte IdCategoria { get; set; }
        public byte IdUsuario { get; set; }
        public string NombreCategoria { get; set; }
        public byte Garantia { get; set; }

        public Categoria()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idCategoria"></param>
        /// <param name="idUsuario"></param>
        /// <param name="nombreCategoria"></param>
        /// <param name="garantia"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Categoria(byte idCategoria, byte idUsuario, string nombreCategoria, byte garantia, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdCategoria = idCategoria;
            IdUsuario = idUsuario;
            NombreCategoria = nombreCategoria;
            Garantia = garantia;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="nombreCategoria"></param>
        /// <param name="garantia"></param>
        public Categoria(byte idUsuario, string nombreCategoria, byte garantia)
        {
            IdUsuario = idUsuario;
            NombreCategoria = nombreCategoria;
            Garantia = garantia;
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
