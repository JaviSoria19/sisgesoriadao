using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface ISucursal : IDao<Sucursal>
    {
        Sucursal Get(byte Id);
        DataTable SelectForComboBox();
        void GetBranchForSession(byte IdSucursal);
        string SelectGroupConcatIDForComboBox();
    }
}
