using System;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace ServiceMigracaoEntreDb
{
    public static class Log
    {
        public static void LoggarComEx(string titulo, Exception ex)
        {
            var textLog = new StringBuilder();
            textLog.AppendLine("Titulo: " + titulo);
            textLog.AppendLine("Data: " + DateTime.Now);
            textLog.AppendLine("Message: " + ex.Message);
            textLog.AppendLine("Source: " + ex.Source);
            textLog.AppendLine("StackTrace: " + ex.StackTrace);

            EventLog eventLog = new EventLog();
            eventLog.Source = "ServiceMigracaoEntreDb";
            eventLog.WriteEntry(textLog.ToString(), EventLogEntryType.Error);
        }

        public static void LoggarInsert(string insert)
        {
            var textLog = new StringBuilder();
            textLog.AppendLine("Insert: " + insert);

            EventLog eventLog = new EventLog();
            eventLog.Source = "ServiceMigracaoEntreDb";
            eventLog.WriteEntry(textLog.ToString(), EventLogEntryType.Information);
        }

        public static void InsertLog(string mensagem)
        {
            var mysql = new MySql();
            mysql.Insert(String.Format(ConfigurationManager.AppSettings["LogMySql"].ToString(), mensagem, ConfigurationManager.AppSettings["idEmpresa"]?.ToString()));
            mysql.Fechar();
        }
    }
}
