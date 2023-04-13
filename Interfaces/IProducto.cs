﻿using System;
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
        Producto GetByIdentifierOrCode(string CadenaBusqueda);
        String GetCodeFormatToInsertProducts(int IdLote);
        int GetSubBatchToInsertProducts(int IdLote);
        DataTable SelectProductHistory(string CadenaBusqueda);
        DataTable SelectSoldProducts();
        DataTable SelectLikeSoldProducts(string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin);
        //LOTES
        int InsertBatch(Lote l);
        int UpdateBatch(Lote l);
        Lote GetBatch(int Id);
        DataTable SelectBatch();
        DataTable SelectLikeBatch(string CadenaBusqueda, DateTime FechaInicio, DateTime FechaFin);
        DataTable SelectBatchForComboBox();
        //FIN LOTES
        string InsertTransaction(List<Producto> ListaProductos, int idLote);
    }
}
