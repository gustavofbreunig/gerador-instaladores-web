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

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConfiguraQuartz();

            SeedsNoBD.FazSeedSQLite();
        }
    }
}
