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
<<<<<<< HEAD
        DataTable SelectDetails(int idCotizacion);
=======
>>>>>>> 186ced3ae7a536bb98b1c5c744b781d7fd732b66
    }
}
