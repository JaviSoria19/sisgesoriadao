using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public static class Session
    {
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
        //Cadena de conexión requerida para llamar a la base de datos.
        public static string CadenaConexionBdD { get; set; } = "server=localhost;database=bdventacelular;uid=root;pwd=1234567890;port=3306";
    }
}
