using System;
using System.ServiceProcess;

namespace ServiceMigracaoEntreDb
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                ServiceMigracaoEntreDb service = new ServiceMigracaoEntreDb(args);
                service.TestStartupAndStop(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new ServiceMigracaoEntreDb(args)
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
