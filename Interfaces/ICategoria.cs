using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface ICategoria : IDao<Categoria>
    {
        Categoria Get(byte Id);
        DataTable SelectForComboBox();
        string SelectGroupConcatIDForComboBox();
    }
}
