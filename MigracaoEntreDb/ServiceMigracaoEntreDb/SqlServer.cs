using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ServiceMigracaoEntreDb
{
    public class SqlServer
    {
        private readonly string _stringConexao = ConfigurationManager.AppSettings["StringConexaoSqlServer"].ToString();
        private readonly SqlConnection conexao;

        public SqlServer()
        {
            conexao = new SqlConnection(_stringConexao);
            conexao.Open();
        }

        public DataTable GetDataTable(string query)
        {
            try
            {
                var adp = new SqlDataAdapter(query, conexao);
                var ds = new DataSet();
                adp.Fill(ds);

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Log.LoggarComEx("Obtendo dados do banco SqlServer", ex);
            }
            finally
            {
                conexao.Close();
            }

            return default;
        }

        public void Fechar()
        {
            conexao.Close();
        }
    }
}
