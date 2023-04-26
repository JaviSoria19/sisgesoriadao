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
        Producto GetByCode(string CadenaBusqueda);
        String GetCodeFormatToInsertProducts(int IdLote);
        int GetSubBatchToInsertProducts(int IdLote);
        DataTable SelectProductHistory(string CadenaBusqueda);
        DataTable SelectSoldProducts();
        DataTable SelectLikeSoldProducts(string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin);
        DataTable SelectProductNamesForComboBox();
        DataTable SelectProductIDandNamesForAutoCompleteBox();
        //LOTES
        int InsertBatch(Lote l);
        int UpdateBatch(Lote l);
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
    }
}
