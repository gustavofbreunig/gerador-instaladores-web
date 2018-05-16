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
            //busca instaladores não iniciados

            //faz um por um


            return Task.CompletedTask;
        }
    }
}
