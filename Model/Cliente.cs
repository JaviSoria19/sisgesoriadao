using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class Cliente:BaseClass
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public string NumeroCelular { get; set; }
        public string NumeroCI { get; set; }
        public Cliente()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="nombre"></param>
        /// <param name="numeroCelular"></param>
        /// <param name="numeroCI"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Cliente(int idCliente, string nombre, string numeroCelular, string numeroCI, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdCliente = idCliente;
            Nombre = nombre;
            NumeroCelular = numeroCelular;
            NumeroCI = numeroCI;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="numeroCelular"></param>
        /// <param name="numeroCI"></param>
        public Cliente(string nombre, string numeroCelular, string numeroCI)
        {
            Nombre = nombre;
            NumeroCelular = numeroCelular;
            NumeroCI = numeroCI;
        }
        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="idCliente"></param>
        public Cliente(int idCliente)
        {
            IdCliente = idCliente;
        }
    }
}
