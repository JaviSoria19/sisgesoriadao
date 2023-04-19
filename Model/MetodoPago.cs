using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class MetodoPago
    {
        public int IdVenta { get; set; }
        public double MontoUSD { get; set; }
        public double MontoBOB { get; set; }
        public byte Tipo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public MetodoPago()
        {

        }
        public MetodoPago(int idVenta, double montoUSD, double montoBOB, byte tipo, DateTime fechaRegistro)
        {
            IdVenta = idVenta;
            MontoUSD = montoUSD;
            MontoBOB = montoBOB;
            Tipo = tipo;
            FechaRegistro = fechaRegistro;
        }
        public MetodoPago(double montoUSD, double montoBOB, byte tipo)
        {
            MontoUSD = montoUSD;
            MontoBOB = montoBOB;
            Tipo = tipo;
        }
        public MetodoPago(double montoUSD, double montoBOB, byte tipo, DateTime fechaRegistro)
        {
            MontoUSD = montoUSD;
            MontoBOB = montoBOB;
            Tipo = tipo;
            FechaRegistro = fechaRegistro;
        }
        public MetodoPago(int idVenta)
        {
            IdVenta = idVenta;
        }
    }
}
