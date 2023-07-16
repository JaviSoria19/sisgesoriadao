using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface IEmpleado : IDao<Empleado>
    {
        Empleado Get(byte Id);
        DataTable SelectEmployeesWithoutUsers();
        int UpdateCreatedUser(Empleado e);
    }
}
