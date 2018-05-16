using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeradorInstaladores.Infra
{
    public enum StatusCompilacao
    {
        NaoIniciado = 0,
        Compilando = 1,
        Terminado = 2,
        Erro = 3
    }
}
