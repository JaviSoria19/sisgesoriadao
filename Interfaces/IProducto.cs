using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Model;
using System.Data;
namespace sisgesoriadao.Interfaces
{
    public interface IProducto : IDao<Producto>
    {
        Producto Get(int Id);
        Producto GetBySNorIMEI(string CadenaBusqueda);
        DataTable SelectSoldProducts();
        DataTable SelectLikeSoldProducts(string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin);
    }
}
