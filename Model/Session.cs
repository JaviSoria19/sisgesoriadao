using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public static class Session
    {
        //Cadena de conexión requerida para llamar a la base de datos.
        public static string CadenaConexionBdD { get; set; } = "server=localhost;database=bdventacelular;uid=root;pwd=1234567890;port=3306";
        public static string VersionApp { get; set; } = "v. 1.2.1.1";
        //Atributo indispensable para manejar la totalidad del sistema.
        public static byte IdUsuario { get; set; }
        //Atributo de referencia para dar a conocer al usuario que ha iniciado sesión correctamente.
        public static string NombreUsuario { get; set; }
        //Atributo requerido para los permisos del usuario
        public static byte Rol { get; set; }
        //Atributos requeridos meramente para realizar la(s) venta(s) y también para desplegarlo en la interfaz del usuario.
        public static byte Sucursal_IdSucursal { get; set; }
        public static string Sucursal_NombreSucursal { get; set; }
        public static string Sucursal_Direccion { get; set; }
        public static string Sucursal_Correo { get; set; }
        public static string Sucursal_Telefono { get; set; }
        //Atributos requeridos para regular el tipo de cambio y el límite de descuento de productos debajo del costo establecido.
        public static double Ajuste_Cambio_Dolar { get; set; }
        public static byte Ajuste_Limite_Descuento { get; set; }
        public static int IdVentaDetalle { get; set; } = 0;
        public static int IdCliente { get; set; } = 0;
    }
}
