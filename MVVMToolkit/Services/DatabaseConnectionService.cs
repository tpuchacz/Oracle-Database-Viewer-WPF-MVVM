using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using MVVMToolkit;
using MVVMToolkit.Services;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Linq.Expressions;

namespace MVVMToolkit.Services;

public partial class DatabaseConnectionService : ObservableRecipient, IDatabaseConnectionService
{
    private string? _connectionString;
    private OracleCredential? _credential;
    private List<OracleDataAdapter> _adapters = new List<OracleDataAdapter>();

    public List<OracleDataAdapter> Adapters
    {
        get { return _adapters; }
        set { _adapters = value; }
    }

    public bool CheckCredentials(string connectionString, OracleCredential credential)
    {
            using (OracleConnection connection = new OracleConnection(connectionString, credential))
            {
                try
                {
                    connection.Open();
                    _connectionString = connectionString;
                    _credential = credential;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
    }

    public List<string> GetTables()
    {
        List<string> tableNames;
        using (OracleConnection connection = new OracleConnection(_connectionString, _credential))
        {
            using (OracleCommand cmd = connection.CreateCommand())
            {   
                connection.Open();
                cmd.CommandText = "select table_name from USER_TABLES";
                cmd.CommandType = CommandType.Text;
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    tableNames = new List<string>();
                    while (reader.Read())
                    {
                        tableNames.Add(reader.GetString(0));
                    }
                }
            }
        }
        return tableNames;
    }

    public List<DataTable> FillTables(List<string> tableNames)
    {
        OracleDataAdapter adapter;
        List<DataTable> _dataTables;
        DataSet _ds;
        DataTable _dt;
        using (OracleConnection connection = new OracleConnection(_connectionString, _credential))
        {
            _dataTables = new List<DataTable>();
            connection.Open();
            for (int i = 0; i < tableNames.Count; ++i)
            {
                _ds = new DataSet(tableNames.ElementAt(i));
                _dt = new DataTable();
                _dt = _ds.Tables.Add(tableNames.ElementAt(i) + " Table");
                string cmdString = $"select * from {tableNames.ElementAt(i)}";
                adapter = new OracleDataAdapter(cmdString, connection);
                OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
                adapter.Fill(_dt);
                var cols = new DataColumn[_dt.Columns.Count];
                for(int j = 0; j < cols.Length; j++)
                {
                    if (!_dt.Columns[j].AllowDBNull)
                        cols[j] = _dt.Columns[j];
                }
                _dt.PrimaryKey = cols;
                _dataTables.Add(_dt);
                adapter.InsertCommand = builder.GetInsertCommand();
                try
                {
                    if (_dt.Columns.Count > 0)
                    {
                        adapter.DeleteCommand = builder.GetDeleteCommand();
                        adapter.UpdateCommand = builder.GetUpdateCommand();
                    }
                }
                catch (Exception){}
                Adapters.Add(adapter);
            }
        }
        return _dataTables;
    }

    public void UpdateTable(int index, DataTable dataTable)
    {
        using (OracleConnection connection = new OracleConnection(_connectionString, _credential))
        {
            connection.Open();
            Adapters.ElementAt(index).InsertCommand.Connection = connection;
            Adapters.ElementAt(index).UpdateCommand.Connection = connection;
            Adapters.ElementAt(index).DeleteCommand.Connection = connection;
            Adapters.ElementAt(index).Update(dataTable);
        }
    }
}
