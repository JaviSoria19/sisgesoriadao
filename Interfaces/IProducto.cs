using sisgesoriadao.Model;
using System;
using System.Collections.Generic;
using System.Data;
namespace sisgesoriadao.Interfaces
{
    public interface IProducto : IDao<Producto>
    {
        Producto Get(int Id);
        Producto GetByCode(string CadenaBusqueda);
        String GetCodeFormatToInsertProducts(int IdLote);
        int GetSubBatchToInsertProducts(int IdLote);
        DataTable SelectProductHistory(string CadenaBusqueda);
        DataTable SelectLikeReporteValorado(string idSucursal, string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin);
        DataTable SelectSoldProducts();
        DataTable SelectLikeSoldProducts(string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin);
        DataTable SelectProductNamesForComboBox();
        DataTable SelectProductIDandNamesForAutoCompleteBox();
        DataTable SelectLikeInventoryFilter(string cadenaBusqueda, string idSucursales, string idCondiciones, string idCategorias, string estados);
        //LOTES
        int InsertBatch(Lote l);
        int UpdateBatch(Lote l);
        int DeleteBatch(Lote l);
        Lote GetBatch(int Id);
        DataTable SelectBatch();
        DataTable SelectLikeBatch(string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin);
        DataTable SelectBatchForComboBox();
        //TRANSFERENCIAS
        string InsertTransaction(List<Producto> ListaProductos, int idLote);
        string UpdateBranchMovementTransaction(List<Producto> ListaProductos, byte idSucursalDestino, string nombreSucursalDestino);
        DataTable SelectPendingProducts();
        int UpdatePendingProduct(int IdProducto);
        DataTable SelectMovementsHistory();
        DataTable SelectLikeMovementsHistory(string SucursalOrigen, string SucursalDestino, DateTime FechaInicio, DateTime FechaFin);
        DataTable SelectMovementsHistory_Details(int IdTransferencia);
        int GetLastMovementFromBranch();
        DataTable SelectProductsFromSale(int IdVenta);
    }
}
