using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Interfaces;
using sisgesoriadao.Model;
using MySql.Data.MySqlClient;//MySql.Data
namespace sisgesoriadao.Implementation
{
    public class ProductoImpl : DataBase, IProducto
    {
        public int Insert(Producto o)
        {
            throw new NotImplementedException();
        }
        public int Update(Producto o)
        {
            throw new NotImplementedException();
        }
        public int Delete(Producto o)
        {
            throw new NotImplementedException();
        }
        public Producto Get(int Id)
        {
            throw new NotImplementedException();
        }            

        public DataTable Select()
        {
            throw new NotImplementedException();
        }
        public DataTable SelectLike(string CadenaBusqueda, DateTime fechaInicio, DateTime fechaFin)
        {
            throw new NotImplementedException();
        }        
    }
}
