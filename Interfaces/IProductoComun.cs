using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Model;
using System.Data;
namespace sisgesoriadao.Interfaces
{
    public interface IProductoComun : IDao<ProductoComun>
    {
        ProductoComun Get(int Id);
        DataTable SelectForComboBox();
        string InsertTransaction(List<ProductoComun> ListaProductosComunes,List<string> ListaDetalles, double Total);
        DataTable SelectLikeCommonProductsSales(DateTime fechaInicio, DateTime fechaFin, string cadenaSucursales, string idUsuarios);
    }
}
