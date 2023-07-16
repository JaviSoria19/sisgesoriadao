namespace sisgesoriadao.Model
{
    public class Ajuste
    {
        public byte IdAjuste { get; set; }
        public double CambioDolar { get; set; }
        public byte LimiteDescuento { get; set; }
        public string FechaActualizacion { get; set; }
        public Ajuste()
        {

        }
        /// <summary>
        /// GET & UPDATE, NO INSERT, NO DELETE.
        /// </summary>
        /// <param name="idAjuste"></param>
        /// <param name="cambioDolar"></param>
        /// <param name="limiteDescuento"></param>
        /// <param name="fechaActualizacion"></param>
        public Ajuste(byte idAjuste, double cambioDolar, byte limiteDescuento, string fechaActualizacion)
        {
            IdAjuste = idAjuste;
            CambioDolar = cambioDolar;
            LimiteDescuento = limiteDescuento;
            FechaActualizacion = fechaActualizacion;
        }
    }
}
