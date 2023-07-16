using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;
namespace sisgesoriadao.Interfaces
{
    public interface IProductoComun : IDao<ProductoComun>
    {
        ProductoComun Get(int Id);
        DataTable SelectForComboBox();
        string InsertTransaction(List<ProductoComun> ListaProductosComunes, List<string> ListaDetalles, double Total);
        DataTable SelectLikeCommonProductsSales(DateTime fechaInicio, DateTime fechaFin, string cadenaSucursales, string idUsuarios);
    }
}
