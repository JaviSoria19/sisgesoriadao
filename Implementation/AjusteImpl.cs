using MySql.Data.MySqlClient;//MySql.Data
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using System;
using System.Data;
namespace sisgesoriadao.Implementation
{
    public class AjusteImpl : DataBase, IAjuste
    {
        public Ajuste Get()
        {
            Ajuste a = null;
            string query = @"SELECT idAjustes, cambio_dolar, limite_descuento,
            intervalo_hora, tema_predeterminado, disenho_boleta, IFNULL(fechaActualizacion,'-')
            FROM Ajustes WHERE idAjustes=1";
            MySqlCommand command = CreateBasicCommand(query);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    a = new Ajuste(byte.Parse(dt.Rows[0][0].ToString()),    /*idAjustes*/
                        double.Parse(dt.Rows[0][1].ToString()),             /*cambio_dolar*/
                        byte.Parse(dt.Rows[0][2].ToString()),               /*limite_descuento*/
                        byte.Parse(dt.Rows[0][3].ToString()),               /*intervalo_hora*/
                        byte.Parse(dt.Rows[0][4].ToString()),               /*tema_predeterminado*/
                        byte.Parse(dt.Rows[0][5].ToString()),               /*disenho_boleta*/
                        dt.Rows[0][6].ToString());                          /*fechaActualizacion*/
                    Session.Ajuste_Cambio_Dolar = a.CambioDolar;
                    Session.Ajuste_Limite_Descuento = a.LimiteDescuento;
                    Session.IntervaloHora = a.IntervaloHora;
                    Session.TemaPredeterminado = a.TemaPredeterminado;
                    Session.DisenhoBoleta = a.DisenhoBoleta;
                    Session.ObtenerPrimeraEtiquetadoraDYMO();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return a;
        }
        public int Update(Ajuste a)
        {
            string query1 = @"UPDATE Ajustes 
                SET cambio_dolar=@cambio_dolar, 
                    limite_descuento=@limite_descuento, 
                    intervalo_hora=@intervalo_hora, 
                    tema_predeterminado=@tema_predeterminado, 
                    disenho_boleta=@disenho_boleta, 
                    fechaActualizacion = CURRENT_TIMESTAMP 
                WHERE idAjustes = 1;";

            string query2 = @"UPDATE Producto p
                JOIN Ajustes a ON a.idAjustes = 1
                SET 
                    p.costoBOB = p.costoUSD * a.cambio_dolar,
                    p.precioVentaBOB = p.precioVentaUSD * a.cambio_dolar
                WHERE p.estado = 1;";

            try
            {
                var cmd1 = CreateBasicCommand(query1);
                cmd1.Parameters.AddWithValue("@cambio_dolar", a.CambioDolar);
                cmd1.Parameters.AddWithValue("@limite_descuento", a.LimiteDescuento);
                cmd1.Parameters.AddWithValue("@intervalo_hora", a.IntervaloHora);
                cmd1.Parameters.AddWithValue("@tema_predeterminado", a.TemaPredeterminado);
                cmd1.Parameters.AddWithValue("@disenho_boleta", a.DisenhoBoleta);

                int result1 = ExecuteBasicCommand(cmd1);

                var cmd2 = CreateBasicCommand(query2);
                int result2 = ExecuteBasicCommand(cmd2);

                return result1 + result2;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int Delete(Ajuste o)
        {
            throw new NotImplementedException();
        }
        public int Insert(Ajuste o)
        {
            throw new NotImplementedException();
        }
        public DataTable Select()
        {
            throw new NotImplementedException();
        }
        public DataTable SelectLike(string CadenaBusqueda, DateTime fechaInicio, DateTime fechaFin)
        {
            throw new NotImplementedException();
        }
    }
}
