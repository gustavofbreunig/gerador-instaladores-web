using GeradorInstaladores.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;

namespace GeradorInstaladores
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private void ConfiguraQuartz()
        {
            IJobDetail job = JobBuilder.Create<JobCriaInstalador>()
                .WithIdentity("JobCriaInstalador", "group1") // name "myJob", group "group1"
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(60)
                    .RepeatForever())
                .Build();

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }

        private void SeedDados()
        {
            //faz o seed dos modelos
            using (var db = new GeradorInstaladoresContext())
            {
                int modelosCadastrados = db.ModelosEquipamentos.Count();

                if (modelosCadastrados == 0)
                {

                    db.ModelosEquipamentos.Add(new ModeloEquipamento()
                    {
                        NomeModelo = "RICOH Aficio MP C2550",
                        PastaDriverX86 = "mpc2050_x86",
                        PastaDriverX64 = "mpc2050_x64",
                        ArquivoINF = "OEMSETUP.INF",
                        NomeDriver = "RICOH Aficio MP C2550 PCL 5c"
                    });

                    db.ModelosEquipamentos.Add(new ModeloEquipamento()
                    {
                        NomeModelo = "Ricoh MP 305",
                        PastaDriverX64 = "mp305_x64",
                        PastaDriverX86 = "mp305_x86",
                        ArquivoINF = "OEMSETUP.INF",
                        NomeDriver = "RICOH MP 305+ PCL 5e"
                    });

                    db.ModelosEquipamentos.Add(new ModeloEquipamento()
                    {
                        NomeModelo = "RICOH Aficio MP 201",
                        PastaDriverX64 = "mp201_x64",
                        PastaDriverX86 = "mp201_x86",
                        ArquivoINF = "OEMSETUP.INF",
                        NomeDriver = "RICOH Aficio MP 201 PCL 5e"
                    });
                }

                int definicoesGravadas = db.DefinicoesGerais.Count();

                if (definicoesGravadas == 0)
                {
                    db.DefinicoesGerais.Add(new DefinicoesGerais()
                    {
                        PastaDrivers = @"D:\temp\instaladorImpressoras",
                        PastaINNO = @"C:\Program Files (x86)\Inno Setup 5"
                    });
                }

                db.SaveChanges();
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConfiguraQuartz();

            SeedDados();
        }
    }
}
