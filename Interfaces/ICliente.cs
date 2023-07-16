using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface ICliente : IDao<Cliente>
    {
        Cliente Get(int Id);
        Cliente GetByCIorCelular(string CadenaBusqueda);
        DataTable SelectCustomerNamesForComboBox();
        Cliente GetFromSale(int idVenta);
        int UpdateSaleCustomer(Cliente c, int idVenta);
    }
}
