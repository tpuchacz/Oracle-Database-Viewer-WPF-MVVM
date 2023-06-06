using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVMToolkit.Models;
using MVVMToolkit.Services;
using MVVMToolkit.ViewModels;
using MVVMToolkit;

namespace MVVMToolkit.ViewModels;

public partial class DatabaseViewModel : BaseViewModel
{
    [ObservableProperty]
    private string? _errorMsg;

    [ObservableProperty]
    private string? _searchBox;

    [ObservableProperty]
    private DatabaseModel _dbModel;

    private readonly IDatabaseConnectionService _databaseConnectionService;

    public DatabaseViewModel(DatabaseModel databaseModel, IDatabaseConnectionService databaseConnectionService)
    {
        DbModel = databaseModel;
        _databaseConnectionService = databaseConnectionService;
        DbModel.TableNames = _databaseConnectionService.GetTables();
        DbModel.TableList = _databaseConnectionService.FillTables(DbModel.TableNames);
        DbModel.SelectedTable = DbModel.TableList.ElementAt(DbModel.SelectedIndex);
    }

    [RelayCommand]
    public void RejectChanges()
    {
        DbModel.SelectedTable.RejectChanges();
    }
    [RelayCommand]
    public void AcceptChanges()
    {
        try
        {
            _databaseConnectionService.UpdateTable(DbModel.SelectedIndex, DbModel.SelectedTable);
            DbModel.SelectedTable.AcceptChanges();
            ErrorMsg = "";
        }
        catch(OracleException ex)
        {
            ErrorMsg = ex.Message;
        }
    }
    [RelayCommand]
    public void AddRow()
    {
        DataRow dr = DbModel.SelectedTable.NewRow();
        DbModel.SelectedTable.Rows.Add(dr);
    }
    [RelayCommand]
    public void Search()
    {
        if(!SearchBox.Equals(string.Empty))
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn column in DbModel.SelectedTable.Columns)
            {
                sb.AppendFormat("CONVERT({0}, System.String) LIKE '%{1}%' OR ", column.ColumnName, SearchBox);
            }
            sb.Remove(sb.Length - 3, 3);
            DbModel.SelectedTable.DefaultView.RowFilter = sb.ToString();
        }
        else
            DbModel.SelectedTable.DefaultView.RowFilter = null;
    }

    partial void OnSearchBoxChanged(string? oldValue, string? newValue)
    {
        if (newValue.Equals(string.Empty))
            SearchCommand.Execute(null);
    }
}
