using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeradorInstaladores.Infra
{
    public class ObterExeInstalador
    {
        private string _PastaDosExecutaveis;
        private string _NomeInstalador;

        public ObterExeInstalador(string IdInstalador)
        {
            using (var db = new GeradorInstaladoresContext())
            {
                var definicoes_gerais = (from d in db.DefinicoesGerais
                                         select d)
                                        .First();

                this._PastaDosExecutaveis = Path.Combine(definicoes_gerais.PastaDrivers, "Output");
                this._NomeInstalador = db.Instaladores
                    .Where(p => p.IdentificadorUnico == IdInstalador)
                    .First()
                    .ArquivoInstalador;
            }
        }

        public FileInfo RetornaArquivo()
        {
            string pathComArquivo = Path.Combine(_PastaDosExecutaveis, _NomeInstalador);
            FileInfo f = new FileInfo(pathComArquivo);
            return f;
        }
    }
}
