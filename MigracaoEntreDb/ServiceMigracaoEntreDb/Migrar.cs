﻿using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace ServiceMigracaoEntreDb
{
    public class Migrar : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var horaInicio = Convert.ToInt32(ConfigurationManager.AppSettings["HoraInicio"]);
            var horaFim = Convert.ToInt32(ConfigurationManager.AppSettings["HoraFim"]);
            var horaAtual = DateTime.Now.Hour;

            if (!(horaInicio <= horaAtual && horaAtual <= horaFim))
                return Task.CompletedTask;

            var inserts = BuscarSqlServer();

            if (inserts == default)
            {
                Log.InsertLog("Não foram inseridos registros");
                return Task.CompletedTask;
            }

            var countInsert = InsertMySql(inserts);

            Log.InsertLog(String.Format("Lidos {0} / Inseridos {1}", inserts.Count, countInsert));

            return Task.CompletedTask;
        }

        private static int InsertMySql(Dictionary<string, string> inserts)
        {
            var countInsert = 0;
            try
            {
                var mysql = new MySql();
                try
                {
                    foreach (var insert in inserts)
                    {
                        if (!mysql.ExisteRegistro(insert.Key))
                        {
                            mysql.Insert(insert.Value);
                            countInsert++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.LoggarComEx("Inserts no banco SqlServer", ex);
                }
                finally
                {
                    mysql.Fechar();
                }
            }
            catch (Exception ex)
            {
                Log.LoggarComEx("Problema ao conectar no banco SqlServer", ex);
            }

            return countInsert;
        }

        private static Dictionary<string, string> BuscarSqlServer()
        {
            try
            {
                var dbSql = new SqlServer();

                try
                {
                    var dtDados = dbSql.GetDataTable(ConfigurationManager.AppSettings["QuerySqlServer"].ToString());

                    if (dtDados == default)
                        return default;

                    var inserts = MontarInsert(dtDados);

                    return inserts;
                }
                catch (Exception ex)
                {
                    Log.LoggarComEx("Montar os Inserts no banco SqlServer", ex);
                    return default;
                }
            }
            catch (Exception ex)
            {
                Log.LoggarComEx("Problema ao conectar no banco SqlServer", ex);
                return default;
            }
        }

        private static Dictionary<string, string> MontarInsert(DataTable dtDados)
        {
            Dictionary<string, string> columns = new Dictionary<string, string>();
            foreach (DataColumn column in dtDados.Columns)
                columns.Add(column.ColumnName, column.DataType.Name);

            var lineColumn = new List<string>();
            var values = new List<string>();
            var inserts = new Dictionary<string, string>();
            foreach (DataRow line in dtDados.Rows)
            {
                foreach (var item in columns)
                {
                    lineColumn.Add(item.Key);
                    switch (item.Value)
                    {
                        case "Int32":
                            values.Add(line[item.Key].ToString());
                            break;
                        case "DateTime":
                            values.Add(@"'" + ((DateTime)line[item.Key]).ToString("yyyy-MM-dd HH:mm:ss") + @"'");
                            break;
                        case "String":
                            values.Add(@"'" + line[item.Key].ToString().Replace("\"","").Replace("\'", "") + @"'");
                            break;
                        case "Decimal":
                            values.Add(line[item.Key].ToString().Replace(",", "."));
                            break;
                        case "Boolean":
                            values.Add((bool)line[item.Key] ? "true" : "false");
                            break;
                        default:
                            break;
                    }
                }

                if (ConfigurationManager.AppSettings["idEmpresa"]?.ToString() != default)
                {
                    lineColumn.Add("idEmpresa");
                    values.Add(ConfigurationManager.AppSettings["idEmpresa"]?.ToString());
                }

                if (ConfigurationManager.AppSettings["dtRegistro"]?.ToString() == "true")
                {
                    lineColumn.Add("dtRegistro");
                    values.Add(@"'" + (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss") + @"'");
                }

                var queryInsert = String.Format(ConfigurationManager.AppSettings["InsertMySql"].ToString(), String.Join(",", lineColumn), String.Join(",", values));
                inserts.Add(line["NumeroOrdemServico"].ToString(), queryInsert);
                lineColumn.Clear();
                values.Clear();

                if (ConfigurationManager.AppSettings["logInsert"]?.ToString() == "true")
                    Log.LoggarInsert(queryInsert);
            }

            return inserts;
        }
    }
}
