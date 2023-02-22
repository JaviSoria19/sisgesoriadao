using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class Empleado:BaseClass
    {
        public byte IdEmpleado { get; set; }
        public string Nombres { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string NumeroCelular { get; set; }
        public string NumeroCI { get; set; }

        //constructor por defecto.
        public Empleado()
        {

        }
        /// <summary>
        /// GET
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <param name="nombres"></param>
        /// <param name="primerApellido"></param>
        /// <param name="segundoApellido"></param>
        /// <param name="numeroCelular"></param>
        /// <param name="numeroCI"></param>
        /// <param name="estado"></param>
        /// <param name="fechaRegistro"></param>
        /// <param name="fechaActualizacion"></param>
        public Empleado(byte idEmpleado, string nombres, string primerApellido, string segundoApellido, string numeroCelular, string numeroCI, byte estado, DateTime fechaRegistro, string fechaActualizacion)
            :base(estado,fechaRegistro,fechaActualizacion)
        {
            IdEmpleado = idEmpleado;
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
        public Empleado(string nombres, string primerApellido, string segundoApellido, string numeroCelular, string numeroCI)
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
        /// <param name="idEmpleado"></param>
        public Empleado(byte idEmpleado)
        {
            IdEmpleado = idEmpleado;
        }
    }
}
