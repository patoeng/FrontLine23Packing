using System.Collections.Generic;
using System.Data.OleDb;

namespace FrontLine600_23
{
    public class Database
    {
        private string _connectionString;
        private readonly string _tableName;

        public Database(string tableName, string provider, string database)
        {
            SetConnectionString(provider,database);
            _tableName = tableName;
        }
        public Database(string tableName)
        {
            _tableName = tableName;
        }

        public void SetConnectionString(string provider, string database)
        {
            _connectionString = $"Provider = {provider};Data Source ={database}; ";
        }
        public Dictionary<string, string> Data;

        public bool LoadByArticle(string articleNumber)
        {
            using (OleDbConnection myConnection = new OleDbConnection())
            {
                myConnection.ConnectionString = _connectionString;
                var queryString = $"select * from {_tableName} where Art_number='{articleNumber}'";
                try
                {
                    OleDbCommand command = new OleDbCommand(queryString, myConnection);
                    command.Connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();
                    if (reader == null) return false;
                    if (reader.Read())
                    {
                        Data = new Dictionary<string, string>();
                        for (int y = 0; y < reader.FieldCount; y++)
                        {
                            Data.Add(reader.GetName(y), reader[y].ToString());
                        }
                        return true;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            return false;
        }

        public bool LoadByReference(string reference)
        {
            using (OleDbConnection myConnection = new OleDbConnection())
            {
                myConnection.ConnectionString = _connectionString;
                var queryString = $"select * from {_tableName} where Reference='{reference}'";
                try
                {
                    OleDbCommand command = new OleDbCommand(queryString, myConnection);
                    command.Connection.Open();
                    OleDbDataReader reader = command.ExecuteReader();

                    if (reader == null) return false;
                    if (reader.Read())
                    {
                        Data = new Dictionary<string, string>();
                        for (int y = 0; y < reader.FieldCount; y++)
                        {
                            Data.Add(reader.GetName(y), reader[y].ToString());
                        }
                        return true;
                    }
                }
                catch
                {
                    // ignored
                }
            }
            return false;
        }
        public bool Update(string column, string value, string keyColumn, string keyValue)
        {
            using (OleDbConnection myConnection = new OleDbConnection())
            {
                myConnection.ConnectionString = _connectionString;
                var queryString = $"Update {_tableName} Set {column}='{value}'  where {keyColumn}='{keyValue}'";

                try
                {
                    OleDbCommand command = new OleDbCommand(queryString, myConnection);

                    command.Connection.Open();
                    var exec = command.ExecuteNonQuery();
                    command.Connection.Close();
                    return true;
                }
                catch
                {
                    // ignored
                }
            }
            return false;

        }
    }
}
