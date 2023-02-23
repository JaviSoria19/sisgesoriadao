﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sisgesoriadao.Model;
using System.Data;

namespace sisgesoriadao.Interfaces
{
    public interface IUsuario:IDao<Usuario>
    {
        Usuario Get(byte Id);
        Usuario Login(string nombreUsuario, string contrasenha);
    }
}
