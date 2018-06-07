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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                log.Info("JobCriaInstalador.Execute iniciado");

                string PastaDrivers, PastaINNO, AppName;

                //busca instaladores não iniciados
                using (var db = new GeradorInstaladoresContext())
                {
                    if (db.DefinicoesGerais.Count() == 0)
                    {
                        log.Error("Definicoes gerais não configuradas");
                        return Task.CompletedTask;
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
                    CriadorInstalador criador = new CriadorInstalador(instalador, PastaDrivers, PastaINNO, AppName);

                    criador.OnMensagemProgresso += Criador_OnMensagemProgresso;
                    criador.OnErro += Criador_OnErro;
                    criador.OnConclusao += Criador_OnConclusao;
                }

                log.Info("JobCriaInstalador.Execute finalizado");               
            }
            catch (Exception e)
            {
                //loga o erro, não é possível exibir nenhuma mensagem ao user, já que este job roda no background
                log.Error(e);
            }

            return Task.CompletedTask;
        }

        private void Criador_OnConclusao(object sender, ProgressoEventArgs e)
        {
            using (var db = new GeradorInstaladoresContext())
            {
                var instalador = db.Instaladores.Find(e.IdInstalador);

                instalador.Status = (int)StatusCompilacao.Terminado;
                instalador.MensagensProgresso += Environment.NewLine + "Concluído!";
                instalador.ArquivoInstalador = e.Mensagem;
                db.SaveChanges();
            }
        }

        private void Criador_OnErro(object sender, ProgressoEventArgs e)
        {
            using (var db = new GeradorInstaladoresContext())
            {
                var instalador = db.Instaladores.Find(e.IdInstalador);

                instalador.Status = (int)StatusCompilacao.Erro;
                instalador.MensagensProgresso += e.Mensagem;
                db.SaveChanges();
            }
        }

        private void Criador_OnMensagemProgresso(object sender, ProgressoEventArgs e)
        {
            using (var db = new GeradorInstaladoresContext())
            {
                var instalador = db.Instaladores.Find(e.IdInstalador);

                instalador.MensagensProgresso += e.Mensagem;
                db.SaveChanges();
            }
        }
    }
}
