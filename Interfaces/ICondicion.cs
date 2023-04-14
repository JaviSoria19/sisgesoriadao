using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface ICondicion :IDao<Condicion>
    {
        Condicion Get(byte Id);
        DataTable SelectForComboBox();
    }
}
