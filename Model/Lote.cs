using System;

namespace sisgesoriadao.Model
{
    public class Lote : BaseClass
    {
        public int IdLote { get; set; }
        public byte IdUsuario { get; set; }
        public string CodigoLote { get; set; }
        public Lote()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idLote"></param>
        /// <param name="idUsuario"></param>
        /// <param name="codigoLote"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Lote(int idLote, byte idUsuario, string codigoLote, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdLote = idLote;
            IdUsuario = idUsuario;
            CodigoLote = codigoLote;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="codigoLote"></param>
        public Lote(byte idUsuario, string codigoLote)
        {
            IdUsuario = idUsuario;
            CodigoLote = codigoLote;
        }
        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="idLote"></param>
        public Lote(int idLote)
        {
            IdLote = idLote;
        }
    }
}
