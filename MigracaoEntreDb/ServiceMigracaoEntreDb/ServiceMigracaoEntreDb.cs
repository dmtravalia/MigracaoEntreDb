using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace ServiceMigracaoEntreDb
{
    public partial class ServiceMigracaoEntreDb : ServiceBase
    {
        public StdSchedulerFactory factory;
        public IScheduler scheduler;

        public ServiceMigracaoEntreDb(string[] args)
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            VerificarAcessoSqlServer();

            VerificarAcessoMySql();

            factory = new StdSchedulerFactory();
            scheduler = factory.GetScheduler().Result;
            scheduler.Start();

            IJobDetail jobMigrar = JobBuilder.Create<Migrar>().Build();
            ITrigger triggerMigrar = TriggerBuilder.Create()
                .WithSimpleSchedule(x => x
                  .WithIntervalInMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["triggerMigrar"].ToString()))
                  .RepeatForever())
                .StartNow()
                .Build();
            scheduler.ScheduleJob(jobMigrar, triggerMigrar);

#if DEBUG
            do
            {
                Thread.Sleep(100);
            } while (true);
#endif
        }

        private static void VerificarAcessoMySql()
        {
            EventLog eventLog = new EventLog();
            eventLog.Source = "ServiceMigracaoEntreDb";
            try
            {
                var mysql = new MySql();
                mysql.Fechar();

                eventLog.WriteEntry("Acesso Ok do MySql", EventLogEntryType.Information);
            }
            catch (Exception)
            {
                eventLog.WriteEntry("Acesso Falha do MySql", EventLogEntryType.Error);
            }
        }

        private static void VerificarAcessoSqlServer()
        {
            EventLog eventLog = new EventLog();
            eventLog.Source = "ServiceMigracaoEntreDb";
            try
            {
                var sqlServer = new SqlServer();
                sqlServer.Fechar();

                eventLog.WriteEntry("Acesso Ok do SqlServer", EventLogEntryType.Information);
            }
            catch (Exception)
            {
                eventLog.WriteEntry("Acesso Falha do SqlServer", EventLogEntryType.Error);
            }
        }

        protected override void OnStop()
        {
            if (scheduler.IsStarted)
            {
                scheduler.Shutdown();
            }
        }

        internal void TestStartupAndStop(string[] args)
        {
            OnStart(null);
        }
    }
}
