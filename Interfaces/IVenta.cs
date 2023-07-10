 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface IVenta:IDao<Venta>
    {
        string InsertTransaction(Venta venta,List<Producto> ListaProductos, List<double> ListaDescuentosPorcentaje, List<byte> ListaGarantias,List<MetodoPago> ListaMetodosPago, Cliente cliente);
        string GetTodaySales(DateTime FechaHoy);
        (double, double) GetCashAmounts();
        string GetTodayProducts(DateTime FechaHoy);
        DataTable SelectSalesWithPendingBalanceFromBranch();
        DataTable SelectLikeReporteUtilidades(DateTime fechaInicio, DateTime fechaFin, string idSucursales, string idCategorias, string idUsuarios);
        DataTable SelectLikeReporteVentasGlobales(DateTime fechaInicio, DateTime fechaFin, string idSucursales, string idCategorias, string idUsuarios);
        DataTable SelectLikeReporteVentasLocales(DateTime fechaInicio, DateTime fechaFin, string productoOCodigo, string clienteoCI);
        DataTable SelectLikeReporteVentasLocalesByID(int idVenta);
        DataTable SelectLikeReporteVentasLocalesDELETED(DateTime fechaInicio, DateTime fechaFin, string productoOCodigo, string clienteoCI);
        DataTable SelectLikeReporteVentasLocalesByIDDELETED(int idVenta);
        DataTable SelectSaleDetails1();
        DataTable SelectSaleDetails2();
        int GetIDAfterInsert();
        DataTable SelectPaymentMethodsFromSale(int IdVenta);
        string InsertPaymentMethodTransaction(int IdVenta, double PagoUSD, double PagoBOB, byte MetodoPago);
        string DeletePaymentMethodTransaction(int IdVenta, int IdMetodoPago, double MontoUSD, double MontoBOB);
        byte GetEstado(int IdVenta);
        string DeleteSaleTransaction(int IdVenta, string Observacion, List<int> ListaIDProductos);
        DataTable SelectSalesWithPendingBalanceByCustomers();
        DataTable SelectAllSalesWithPendingBalanceByCustomers();
        string UpdateProductPriceBeforeSaleTransaction(Venta Venta, Producto Producto, double Descuento, byte Garantia);
    }
}
