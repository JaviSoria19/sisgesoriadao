using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface IUsuario : IDao<Usuario>
    {
        Usuario Get(byte Id);
        Usuario Login(string nombreUsuario, string contrasenha);
        DataTable SelectForComboBox();
        string SelectGroupConcatIDForComboBox();
    }
}
