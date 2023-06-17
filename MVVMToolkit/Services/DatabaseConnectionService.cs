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
    //All connections are in using statement - Dispose method is automatically invoked

    //Credentials get set after connection is established for the first time
    private string? _connectionString;
    private OracleCredential? _credential;
    private List<OracleDataAdapter> _adapters = new List<OracleDataAdapter>();

    public List<OracleDataAdapter> Adapters
    {
        get { return _adapters; }
        set { _adapters = value; }
    }

    //Simple check if credentials are correct by connecting to the DB
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

    //Getting a list of all table names
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
        tableNames.Sort();
        return tableNames;
    }

    //Filling the tables with data; preserving adapters for later;
    //building update commands(had issues with building them later)
    public List<DataTable> FillTables(List<string> tableNames)
    {
        OracleDataAdapter adapter;
        List<DataTable> _dataTables;
        DataSet _ds; //Couldn't bind without DataSet
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
                //There was an exception thrown while updating data with a DataGrid error. Lower line seems to fix it
                builder.ConflictOption = ConflictOption.OverwriteChanges; 
                adapter.Fill(_dt);
                var cols = new DataColumn[_dt.Columns.Count];
                for(int j = 0; j < cols.Length; j++)
                {
                    if (!_dt.Columns[j].AllowDBNull)
                        cols[j] = _dt.Columns[j];
                }
                _dt.PrimaryKey = cols; //Could be done better by also checking which columns are Primary Key
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
            Adapters.ElementAt(index).AcceptChangesDuringUpdate = true;
            //Updating connection; previous one disposed
            Adapters.ElementAt(index).InsertCommand.Connection = connection;
            Adapters.ElementAt(index).UpdateCommand.Connection = connection;
            Adapters.ElementAt(index).DeleteCommand.Connection = connection;
            Adapters.ElementAt(index).Update(dataTable);
        }
    }
}
