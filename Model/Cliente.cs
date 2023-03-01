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
        public string Nombres { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string NumeroCelular { get; set; }
        public string NumeroCI { get; set; }
        public Cliente()
        {

        }
        /// <summary>
        /// GET & UPDATE
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="nombres"></param>
        /// <param name="primerApellido"></param>
        /// <param name="segundoApellido"></param>
        /// <param name="numeroCelular"></param>
        /// <param name="numeroCI"></param>
        public Cliente(int idCliente, string nombres, string primerApellido, string segundoApellido, string numeroCelular, string numeroCI, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            : base(estado, fechaRegistro, fechaActualizacion)
        {
            IdCliente = idCliente;
            Nombres = nombres;
            PrimerApellido = primerApellido;
            SegundoApellido = segundoApellido;
            NumeroCelular = numeroCelular;
            NumeroCI = numeroCI;
        }
        /// <summary>
        /// INSERT
        /// </summary>
        /// <param name="nombres"></param>
        /// <param name="primerApellido"></param>
        /// <param name="segundoApellido"></param>
        /// <param name="numeroCelular"></param>
        /// <param name="numeroCI"></param>
        public Cliente(string nombres, string primerApellido, string segundoApellido, string numeroCelular, string numeroCI)
        {
            Nombres = nombres;
            PrimerApellido = primerApellido;
            SegundoApellido = segundoApellido;
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
