using System;

namespace sisgesoriadao.Model
{
    public class Cotizacion
    {
        public int IdCotizacion { get; set; }
        public byte IdUsuario { get; set; }
        public byte IdSucursal { get; set; }
        public string NombreCliente { get; set; }
        public string NombreEmpresa { get; set; }
        public string Nit { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public DateTime TiempoEntrega { get; set; }
        public DateTime FechaRegistro { get; set; }
        public Cotizacion()
        {

        }
        /// <summary>
        /// GET
        /// </summary>
        /// <param name="idCotizacion"></param>
        /// <param name="idUsuario"></param>
        /// <param name="idSucursal"></param>
        /// <param name="nombreCliente"></param>
        /// <param name="nombreEmpresa"></param>
        /// <param name="nit"></param>
        /// <param name="direccion"></param>
        /// <param name="correo"></param>
        /// <param name="telefono"></param>
        /// <param name="tiempoEntrega"></param>
        /// <param name="fechaRegistro"></param>
        public Cotizacion(int idCotizacion, byte idUsuario, byte idSucursal, string nombreCliente, string nombreEmpresa, string nit, string direccion, string correo, string telefono, DateTime tiempoEntrega, DateTime fechaRegistro)
        {
            IdCotizacion = idCotizacion;
            IdUsuario = idUsuario;
            IdSucursal = idSucursal;
            NombreCliente = nombreCliente;
            NombreEmpresa = nombreEmpresa;
            Nit = nit;
            Direccion = direccion;
            Correo = correo;
            Telefono = telefono;
            TiempoEntrega = tiempoEntrega;
            FechaRegistro = fechaRegistro;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="idSucursal"></param>
        /// <param name="nombreCliente"></param>
        /// <param name="nombreEmpresa"></param>
        /// <param name="nit"></param>
        /// <param name="direccion"></param>
        /// <param name="correo"></param>
        /// <param name="telefono"></param>
        /// <param name="tiempoEntrega"></param>
        public Cotizacion(byte idUsuario, byte idSucursal, string nombreCliente, string nombreEmpresa, string nit, string direccion, string correo, string telefono, DateTime tiempoEntrega)
        {
            IdUsuario = idUsuario;
            IdSucursal = idSucursal;
            NombreCliente = nombreCliente;
            NombreEmpresa = nombreEmpresa;
            Nit = nit;
            Direccion = direccion;
            Correo = correo;
            Telefono = telefono;
            TiempoEntrega = tiempoEntrega;
        }
    }
}
