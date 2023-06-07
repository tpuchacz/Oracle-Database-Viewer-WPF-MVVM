using CommunityToolkit.Mvvm.ComponentModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVMToolkit;

namespace MVVMToolkit.Services;

public interface IDatabaseConnectionService
{
    bool CheckCredentials(string connectionString, OracleCredential credentials);
    List<string> GetTables();
    List<DataTable> FillTables(List<string> tableNames);
    void UpdateTable(int index, DataTable dataTable);
}
