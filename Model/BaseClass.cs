using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisgesoriadao.Model
{
    public class BaseClass
    {
        public byte Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string FechaActualizacion { get; set; }

        //constructor por defecto.
        public BaseClass()
        {

        }

        public BaseClass(byte estado, DateTime fechaRegistro, string fechaActualizacion)
        {
            Estado = estado;
            FechaRegistro = fechaRegistro;
            FechaActualizacion = fechaActualizacion;
        }
    }
}
