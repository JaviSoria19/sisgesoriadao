using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface IVenta:IDao<Venta>
    {
        string InsertTransaction(Venta venta,List<Producto> ListaProductos, List<double> ListaDescuentosPorcentaje, List<Categoria> ListaGarantias,List<MetodoPago> ListaMetodosPago, Cliente cliente);
        string GetTodaySales(DateTime FechaHoy);
        (double, double) GetCashAmounts();
        string GetTodayProducts(DateTime FechaHoy);
    }
}
