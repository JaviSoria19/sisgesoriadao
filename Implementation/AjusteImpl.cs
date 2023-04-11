using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using MySql.Data.MySqlClient;//MySql.Data
namespace sisgesoriadao.Implementation
{
    public class AjusteImpl : DataBase, IAjuste
    {
        public Ajuste Get()
        {
            Ajuste a = null;
            string query = @"SELECT idAjustes, cambio_dolar, limite_descuento, IFNULL(fechaActualizacion,'-') FROM ajustes WHERE idAjustes=1";
            MySqlCommand command = CreateBasicCommand(query);
            try
            {
                DataTable dt = ExecuteDataTableCommand(command);
                if (dt.Rows.Count > 0)
                {
                    a = new Ajuste(byte.Parse(dt.Rows[0][0].ToString()),    /*idAjustes*/
                        double.Parse(dt.Rows[0][1].ToString()),             /*cambio_dolar*/
                        byte.Parse(dt.Rows[0][2].ToString()),               /*limite_descuento*/
                        dt.Rows[0][3].ToString());                          /*fechaActualizacion*/
                    Session.Ajuste_Cambio_Dolar = a.CambioDolar;
                    Session.Ajuste_Limite_Descuento = a.LimiteDescuento;
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
            string query = @"UPDATE ajustes SET 
                cambio_dolar=@cambio_dolar, limite_descuento=@limite_descuento, 
                fechaActualizacion = CURRENT_TIMESTAMP WHERE idAjustes = 1";
            MySqlCommand command = CreateBasicCommand(query);
            command.Parameters.AddWithValue("@cambio_dolar", a.CambioDolar);
            command.Parameters.AddWithValue("@limite_descuento", a.LimiteDescuento);
            try
            {
                return ExecuteBasicCommand(command);
            }
            catch (Exception ex)
            {

                throw ex;
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
