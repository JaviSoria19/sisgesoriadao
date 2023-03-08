using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public static class Session
    {
        public static byte IdUsuario { get; set; }
        public static string NombreUsuario { get; set; }
        public static byte Rol { get; set; }
        public static byte Sucursal_IdSucursal { get; set; }
        public static string Sucursal_NombreSucursal { get; set; }
        public static string Sucursal_Direccion { get; set; }
        public static string Sucursal_correo { get; set; }
    }
}
