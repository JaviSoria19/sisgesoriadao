using sisgesoriadao.Model;
using System.Collections.Generic;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface ICotizacion : IDao<Cotizacion>
    {
        string InsertTransaction(List<Producto> ListaProductos, Cotizacion cotizacion);
        Cotizacion Get(int id);
        DataTable SelectDetails(int idCotizacion);
        DataTable SelectDetails2(int idCotizacion);
        Cotizacion GetLastFromBranch();
    }
}
