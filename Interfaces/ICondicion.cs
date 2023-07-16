using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface ICondicion : IDao<Condicion>
    {
        Condicion Get(byte Id);
        DataTable SelectForComboBox();
        string SelectGroupConcatIDForComboBox();
    }
}
