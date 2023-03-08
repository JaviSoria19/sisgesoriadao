using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class Venta:BaseClass
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public byte IdUsuario { get; set; }
        public double Total { get; set; }
        public byte MetodoPago { get; set; }
        public string Moneda { get; set; }
        public byte IdSucursal { get; set; }
        public Venta()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idVenta"></param>
        /// <param name="idCliente"></param>
        /// <param name="idUsuario"></param>
        /// <param name="total"></param>
        /// <param name="metodoPago"></param>
        /// <param name="moneda"></param>
        /// <param name="idSucursal"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Venta(int idVenta, int idCliente, byte idUsuario, double total, byte metodoPago, string moneda, byte idSucursal, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdVenta = idVenta;
            IdCliente = idCliente;
            IdUsuario = idUsuario;
            Total = total;
            MetodoPago = metodoPago;
            Moneda = moneda;
            IdSucursal = idSucursal;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idUsuario"></param>
        /// <param name="total"></param>
        /// <param name="metodoPago"></param>
        /// <param name="moneda"></param>
        /// <param name="idSucursal"></param>
        public Venta(int idCliente, byte idUsuario, double total, byte metodoPago, string moneda, byte idSucursal)
        {
            IdCliente = idCliente;
            IdUsuario = idUsuario;
            Total = total;
            MetodoPago = metodoPago;
            Moneda = moneda;
            IdSucursal = idSucursal;
        }
        /// <summary>
        /// "DELETE"
        /// </summary>
        /// <param name="idVenta"></param>
        public Venta(int idVenta)
        {
            IdVenta = idVenta;
        }
    }
}
