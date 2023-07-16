using System;

namespace sisgesoriadao.Model
{
    public class ProductoComun : BaseClass
    {
        public int IdProductoComun { get; set; }
        public string NombreProductoComun { get; set; }
        public double PrecioMinimo { get; set; }
        public double PrecioSugerido { get; set; }
        public ProductoComun()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idProductoComun"></param>
        /// <param name="nombreProducto"></param>
        /// <param name="precioMinimo"></param>
        /// <param name="precioSugerido"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public ProductoComun(int idProductoComun, string nombreProducto, double precioMinimo, double precioSugerido, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdProductoComun = idProductoComun;
            NombreProductoComun = nombreProducto;
            PrecioMinimo = precioMinimo;
            PrecioSugerido = precioSugerido;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="nombreProducto"></param>
        /// <param name="precioMinimo"></param>
        /// <param name="precioSugerido"></param>
        public ProductoComun(string nombreProducto, double precioMinimo, double precioSugerido)
        {
            NombreProductoComun = nombreProducto;
            PrecioMinimo = precioMinimo;
            PrecioSugerido = precioSugerido;
        }
        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="idProductoComun"></param>
        public ProductoComun(int idProductoComun)
        {
            IdProductoComun = idProductoComun;
        }
        /// <summary>
        /// CONSTRUCTOR REQUERIDO PARA GENERAR UNA LISTA DE LA VENTA Y REALIZAR LA TRANSACCION.
        /// </summary>
        /// <param name="idProductoComun"></param>
        /// <param name="nombreProductoComun"></param>
        /// <param name="precioSugerido"></param>
        public ProductoComun(int idProductoComun, string nombreProductoComun, double precioSugerido) : this(idProductoComun)
        {
            NombreProductoComun = nombreProductoComun;
            PrecioSugerido = precioSugerido;
        }
    }
}
