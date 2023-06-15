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
        public byte IdCategoria { get; set; }
        public int IdSublote { get; set; }
        public byte IdCondicion { get; set; }
        public byte IdUsuario { get; set; }
        public string CodigoSublote { get; set; }
        public string NombreProducto { get; set; }
        public string Identificador { get; set; }
        public double CostoUSD { get; set; }
        public double CostoBOB { get; set; }
        public double PrecioVentaUSD { get; set; }
        public double PrecioVentaBOB { get; set; }
        public string Observaciones { get; set; }
        public Producto()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idProducto"></param>
        /// <param name="idSucursal"></param>
        /// <param name="idCategoria"></param>
        /// <param name="idSublote"></param>
        /// <param name="idUsuario"></param>
        /// <param name="codigoSublote"></param>
        /// <param name="nombreProducto"></param>
        /// <param name="identificador"></param>
        /// <param name="costoUSD"></param>
        /// <param name="costoBOB"></param>
        /// <param name="precioVentaUSD"></param>
        /// <param name="precioVentaBOB"></param>
        /// <param name="observaciones"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Producto(int idProducto, byte idSucursal, byte idCategoria, int idSublote, byte idCondicion, byte idUsuario, string codigoSublote, string nombreProducto, string identificador, double costoUSD, double costoBOB, double precioVentaUSD, double precioVentaBOB, string observaciones, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdProducto = idProducto;
            IdSucursal = idSucursal;
            IdCategoria = idCategoria;
            IdSublote = idSublote;
            IdCondicion = idCondicion;
            IdUsuario = idUsuario;
            CodigoSublote = codigoSublote;
            NombreProducto = nombreProducto;
            Identificador = identificador;
            CostoUSD = costoUSD;
            CostoBOB = costoBOB;
            PrecioVentaUSD = precioVentaUSD;
            PrecioVentaBOB = precioVentaBOB;
            Observaciones = observaciones;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <param name="idCategoria"></param>
        /// <param name="idSublote"></param>
        /// <param name="idUsuario"></param>
        /// <param name="codigoSublote"></param>
        /// <param name="nombreProducto"></param>
        /// <param name="identificador"></param>
        /// <param name="costoUSD"></param>
        /// <param name="costoBOB"></param>
        /// <param name="precioVentaUSD"></param>
        /// <param name="precioVentaBOB"></param>
        /// <param name="observaciones"></param>
        public Producto(byte idSucursal, byte idCategoria, int idSublote, byte idCondicion, byte idUsuario, string codigoSublote, string nombreProducto, string identificador, double costoUSD, double costoBOB, double precioVentaUSD, double precioVentaBOB, string observaciones)
        {
            IdSucursal = idSucursal;
            IdCategoria = idCategoria;
            IdSublote = idSublote;
            IdCondicion = idCondicion;
            IdUsuario = idUsuario;
            CodigoSublote = codigoSublote;
            NombreProducto = nombreProducto;
            Identificador = identificador;
            CostoUSD = costoUSD;
            CostoBOB = costoBOB;
            PrecioVentaUSD = precioVentaUSD;
            PrecioVentaBOB = precioVentaBOB;
            Observaciones = observaciones;
        }
        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="idProducto"></param>
        public Producto(int idProducto)
        {
            IdProducto = idProducto;
        }
        /// <summary>
        /// CONSTRUCTOR REQUERIDO PARA REALIZAR LA INSERCIÓN DE LA VENTA.
        /// </summary>
        /// <param name="idProducto"></param>
        /// <param name="precioVentaUSD"></param>
        /// <param name="precioVentaBOB"></param>
        public Producto(int idProducto, double precioVentaUSD, double precioVentaBOB) : this(idProducto)
        {
            PrecioVentaUSD = precioVentaUSD;
            PrecioVentaBOB = precioVentaBOB;
        }
    }
}
