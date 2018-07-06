using GeradorInstaladores.Infra;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeradorInstaladores.ConsoleCriadorInstaladores
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static void ConfiguraQuartz()
        {
            IJobDetail job = JobBuilder.Create<JobCriaInstalador>()
                .WithIdentity("JobCriaInstalador", "group1") // name "myJob", group "group1"
                .Build();

            job.JobDataMap["log"] = log;

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(60)
                    .RepeatForever()
                    )
                .Build();

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.ScheduleJob(job, trigger);
            scheduler.Start();
        }

        static void Main(string[] args)
        {
            SeedsNoBD.FazSeedSQLite();

            ConfiguraQuartz();

            while (true)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}
