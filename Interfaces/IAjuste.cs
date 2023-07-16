using sisgesoriadao.Model;

namespace sisgesoriadao.Interfaces
{
    public interface IAjuste : IDao<Ajuste>
    {
        Ajuste Get();
    }
}
