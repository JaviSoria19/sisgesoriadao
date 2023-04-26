using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Model;
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
    }
}
