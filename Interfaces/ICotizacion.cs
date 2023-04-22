using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface ICotizacion : IDao<Cotizacion>
    {
        string InsertTransaction(List<Producto> ListaProductos);
        Cotizacion Get(int id);
    }
}
