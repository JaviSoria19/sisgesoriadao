using System;

namespace sisgesoriadao.Model
{
    public class PagoSublote
    {
        public int IdPagoSublote { get; set; }
        public int idSublote { get; set; }
        public double MontoUSD { get; set; }
        public DateTime FechaRegistro { get; set; }

        public PagoSublote()
        {

        }

        public PagoSublote(int idPagoSublote, int idSublote, double montoUSD, DateTime fechaRegistro)
        {
            IdPagoSublote = idPagoSublote;
            this.idSublote = idSublote;
            MontoUSD = montoUSD;
            FechaRegistro = fechaRegistro;
        }

        public PagoSublote(int idSublote, double montoUSD, DateTime fechaRegistro)
        {
            this.idSublote = idSublote;
            MontoUSD = montoUSD;
            FechaRegistro = fechaRegistro;
        }

        public PagoSublote(double montoUSD, DateTime fechaRegistro)
        {
            MontoUSD = montoUSD;
            FechaRegistro = fechaRegistro;
        }

        public PagoSublote(double montoUSD)
        {
            MontoUSD = montoUSD;
        }
    }
}
