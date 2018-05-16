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
        private void RegistraErro(int IdInstalador, string Erro)
        {
            using (var db = new GeradorInstaladoresContext())
            {
                Instalador instalador = db.Instaladores.Find(IdInstalador);

                instalador.Erro = Erro;
                instalador.Status = (int)StatusCompilacao.Erro;
                db.SaveChanges();
            }
        }

        private int[] InstaladoresACompilar()
        {
            using (var db = new GeradorInstaladoresContext())
            {
                return (
                    from i in db.Instaladores
                    where i.Status == (int)StatusCompilacao.NaoIniciado
                    select i.Id
                        ).ToArray();
            }
        }

        private void AlteraParaCompilando(int[] instaladores)
        {
            using (var db = new GeradorInstaladoresContext())
            {
                foreach (int id in instaladores)
                {
                    Instalador instalador = db.Instaladores.Find(id);

                    instalador.Status = (int)StatusCompilacao.Compilando;
                }

                db.SaveChanges();
            }                
        }

        public Task Execute(IJobExecutionContext context)
        {
            //pega a lista de instaladores a compilar
            int[] paraCompilar = InstaladoresACompilar();

            //muda o status para compilando
            AlteraParaCompilando(paraCompilar);

            //compila os instaladores


            return Task.CompletedTask;
        }
    }
}
