using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class Producto : BaseClass
    {
        public int IdProducto { get; set; }
        public byte IdSucursal { get; set; }
        public byte IdMarca { get; set; }
        public byte IdCategoria { get; set; }
        public string NombreProducto { get; set; }
        public string Color { get; set; }
        public string NumeroSerie { get; set; }
        public double Precio { get; set; }
        public string Moneda { get; set; }
        public byte IdUsuario { get; set; }
        public Producto()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idProducto"></param>
        /// <param name="idSucursal"></param>
        /// <param name="idMarca"></param>
        /// <param name="idCategoria"></param>
        /// <param name="nombreProducto"></param>
        /// <param name="color"></param>
        /// <param name="numeroSerie"></param>
        /// <param name="precio"></param>
        /// <param name="moneda"></param>
        /// <param name="idUsuario"></param>
        public Producto(int idProducto, byte idSucursal, byte idMarca, byte idCategoria, string nombreProducto, string color, string numeroSerie, double precio, string moneda, byte idUsuario, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdProducto = idProducto;
            IdSucursal = idSucursal;
            IdMarca = idMarca;
            IdCategoria = idCategoria;
            NombreProducto = nombreProducto;
            Color = color;
            NumeroSerie = numeroSerie;
            Precio = precio;
            Moneda = moneda;
            IdUsuario = idUsuario;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <param name="idMarca"></param>
        /// <param name="idCategoria"></param>
        /// <param name="nombreProducto"></param>
        /// <param name="color"></param>
        /// <param name="numeroSerie"></param>
        /// <param name="precio"></param>
        /// <param name="moneda"></param>
        /// <param name="idUsuario"></param>
        public Producto(byte idSucursal, byte idMarca, byte idCategoria, string nombreProducto, string color, string numeroSerie, double precio, string moneda, byte idUsuario)
        {
            IdSucursal = idSucursal;
            IdMarca = idMarca;
            IdCategoria = idCategoria;
            NombreProducto = nombreProducto;
            Color = color;
            NumeroSerie = numeroSerie;
            Precio = precio;
            Moneda = moneda;
            IdUsuario = idUsuario;
        }
        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="idProducto"></param>
        public Producto(int idProducto)
        {
            IdProducto = idProducto;
        }
    }
}
