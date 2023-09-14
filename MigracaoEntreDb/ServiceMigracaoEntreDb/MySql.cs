using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;

namespace ServiceMigracaoEntreDb
{
    public class MySql
    {
        private readonly string _stringConexao = ConfigurationManager.AppSettings["StringConexaoMySql"].ToString();
        private readonly MySqlConnection conexao;

        public MySql()
        {
            conexao = new MySqlConnection(_stringConexao);
            conexao.Open();
        }

        public bool ExisteRegistro(string numeroOrdemServico)
        {
            var query = String.Format(ConfigurationManager.AppSettings["QueryVerificarNumeroOrdemServicoMySql"].ToString(), numeroOrdemServico);
            var adp = new MySqlDataAdapter(query, conexao);
            var ds = new DataSet();
            adp.Fill(ds);

            return ds.Tables[0].Rows.Count > 0;
        }

        public int Insert(string query)
        {
            var comand = new MySqlCommand(query)
            {
                Connection = conexao
            };
            var sqlDataReader = comand.ExecuteReader();
            sqlDataReader.Close();

            return sqlDataReader.RecordsAffected;
        }

        public void Fechar()
        {
            conexao.Close();
        }
    }
}