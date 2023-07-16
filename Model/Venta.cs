﻿using System;

namespace sisgesoriadao.Model
{
    public class Venta : BaseClass
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public byte IdUsuario { get; set; }
        public byte IdSucursal { get; set; }
        public double TotalUSD { get; set; }
        public double TotalBOB { get; set; }
        public double SaldoUSD { get; set; }
        public double SaldoBOB { get; set; }
        public string Observaciones { get; set; }

        public Venta()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idVenta"></param>
        /// <param name="idCliente"></param>
        /// <param name="idUsuario"></param>
        /// <param name="idSucursal"></param>
        /// <param name="totalUSD"></param>
        /// <param name="totalBOB"></param>
        /// <param name="saldo"></param>
        /// <param name="observaciones"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Venta(int idVenta, int idCliente, byte idUsuario, byte idSucursal, double totalUSD, double totalBOB, double saldoUSD, double saldoBOB, string observaciones, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdVenta = idVenta;
            IdCliente = idCliente;
            IdUsuario = idUsuario;
            IdSucursal = idSucursal;
            TotalUSD = totalUSD;
            TotalBOB = totalBOB;
            SaldoUSD = saldoUSD;
            SaldoBOB = saldoBOB;
            Observaciones = observaciones;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idUsuario"></param>
        /// <param name="idSucursal"></param>
        /// <param name="totalUSD"></param>
        /// <param name="totalBOB"></param>
        /// <param name="saldo"></param>
        /// <param name="observaciones"></param>
        public Venta(int idCliente, byte idUsuario, byte idSucursal, double totalUSD, double totalBOB, double saldoUSD, double saldoBOB, string observaciones)
        {
            IdCliente = idCliente;
            IdUsuario = idUsuario;
            IdSucursal = idSucursal;
            TotalUSD = totalUSD;
            TotalBOB = totalBOB;
            SaldoUSD = saldoUSD;
            SaldoBOB = saldoBOB;
            Observaciones = observaciones;
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
