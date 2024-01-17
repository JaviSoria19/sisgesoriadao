using System;

namespace sisgesoriadao.Model
{
    public class ProductoEnCola
    {
        public string CodigoSublote { get; set; }
        public string NombreProducto { get; set; }
        public string Operacion { get; set; }
        public ProductoEnCola()
        {

        }
        public ProductoEnCola(string codigoSublote, string nombreProducto, string operacion)
        {
            CodigoSublote = codigoSublote;
            NombreProducto = nombreProducto;
            Operacion = operacion;
        }
    }
}
