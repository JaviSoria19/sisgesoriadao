using sisgesoriadao.Model;
using System.Collections.Generic;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface ICotizacion : IDao<Cotizacion>
    {
        string InsertTransaction(List<Producto> ListaProductos, Cotizacion cotizacion, List<short> ListaCantidades);
        string UpdateTransaction(List<Producto> ListaProductos, Cotizacion cotizacion, List<short> ListaCantidades);
        string DeleteAfterQuotationProductTransaction(Cotizacion cotizacion, int IdProducto);
        Cotizacion Get(int id);
        DataTable SelectDetails(int idCotizacion);
        DataTable SelectDetails2(int idCotizacion);
        DataTable SelectQuotationCustomerNamesForAutoCompleteBox();
        Cotizacion GetLastFromBranch();
        byte GetEstado(int idCotizacion);
    }
}
