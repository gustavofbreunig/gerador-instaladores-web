using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeradorInstaladores.Infra
{
    [DisallowConcurrentExecution]
    public class JobCriaInstalador : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            string PastaDrivers, PastaINNO, AppName;            

            //busca instaladores não iniciados
            using (var db = new GeradorInstaladoresContext())
            {
                if (db.DefinicoesGerais.Count() == 0)
                {
                    throw new Exception("Definicoes gerais não configuradas");
                }

                var definicoes_gerais = (from d in db.DefinicoesGerais
                                         select d)
                                        .First();

                PastaDrivers = definicoes_gerais.PastaDrivers;
                PastaINNO = definicoes_gerais.PastaINNO;
                AppName = definicoes_gerais.AppName;

                var instaladores = from i in db.Instaladores
                                   where i.Status == (int)StatusCompilacao.NaoIniciado
                                   select i;

                foreach (var instalador in instaladores)
                {
                    instalador.Status = (int)StatusCompilacao.Compilando;
                }

                db.SaveChanges();
            }

            List<Instalador> instaladores_a_compilar;

            //pega os instaladores a compilar
            using (var db = new GeradorInstaladoresContext())
            {
                instaladores_a_compilar = (from i in db.Instaladores
                                           where i.Status == (int)StatusCompilacao.Compilando
                                           select i)
                                           .ToList();
            }

            //compila um por um
            foreach (var instalador in instaladores_a_compilar)
            {

            }

            return Task.CompletedTask;
        }
    }
}
