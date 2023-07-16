using sisgesoriadao.Model;
using System;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface ICaja : IDao<Caja>
    {
        Caja Get(int Id);
        Caja GetByBranch();
        DataTable SelectPendingCashFromBranch();
        string UpdateClosePendingCashTransaction(Caja c);
        DataTable SelectDetails(Caja c);
        DataTable SelectLikeByCashTypeAndUsers(string tipoCajas, string idUsuarios, DateTime fechaInicio, DateTime fechaFin);
    }
}
