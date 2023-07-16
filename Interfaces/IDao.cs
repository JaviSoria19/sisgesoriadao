using System;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface IDao<Objeto>
    {
        int Insert(Objeto o);
        int Update(Objeto o);
        int Delete(Objeto o);
        DataTable Select();
        DataTable SelectLike(string CadenaBusqueda, DateTime fechaInicio, DateTime fechaFin);
        //para filtros de fechas por ejemplo, se podrá utilizar DataTable SelectFiltro(uno_o_mas_parametros_de_entrada);
    }
}
