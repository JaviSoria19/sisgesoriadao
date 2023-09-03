namespace sisgesoriadao.Model
{
    public static class Session
    {
        //Cadena de conexión requerida para llamar a la base de datos.
        public static string CadenaConexionBdD { get; set; } = "server=localhost;database=bdventacelular;uid=root;pwd=1234567890;port=3306";
        public static string VersionApp { get; set; } = "v. 1.3.6.2";
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
        public static byte NumeroFormatoFecha { get; set; } = 2;
        public static byte IntervaloHora { get; set; } = 1;

        public static string FormatoFechaMySql(string MySqlAtributoFecha)
        {
            switch (NumeroFormatoFecha)
            {

                case 1:
                    /*DD/MM/AAAA*/
                    return "DATE_FORMAT(" + MySqlAtributoFecha + " + INTERVAL " + IntervaloHora + " HOUR,'%d/%m/%Y')";
                case 2:
                    /*DD/MM/AAAA HH:MM*/
                    return "DATE_FORMAT(" + MySqlAtributoFecha + " + INTERVAL " + IntervaloHora + " HOUR,'%d/%m/%Y %T')";
                case 3:
                    /*YYYY/MM/DD*/
                    return "DATE_FORMAT(" + MySqlAtributoFecha + " + INTERVAL " + IntervaloHora + " HOUR,'%Y/%m/%d')";
                case 4:
                    /*YYYY/MM/DD HH:MM*/
                    return "DATE_FORMAT(" + MySqlAtributoFecha + " + INTERVAL " + IntervaloHora + " HOUR,'%Y/%m/%d %T')";
                default:
                    return MySqlAtributoFecha;
            }
        }
    }
}
