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
    private readonly IDatabaseConnectionService _databaseConnectionService;

    public DatabaseViewModel(DatabaseModel databaseModel, IDatabaseConnectionService databaseConnectionService)
    {
        DbModel = databaseModel;
        _databaseConnectionService = databaseConnectionService;
        using(BackgroundWorker bgw = new BackgroundWorker())
        {
            bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
            bgw.DoWork += LoadTables;
            bgw.RunWorkerAsync();
        }
    }

    [ObservableProperty]
    private string? _errorMsg;

    [ObservableProperty]
    private string? _searchBox;

    [ObservableProperty]
    private DatabaseModel _dbModel;

    [ObservableProperty]
    private int _progress = 0;

    [ObservableProperty]
    private string _isDataGridHidden = "Hidden";

    [ObservableProperty]
    private string _isProgressBarHidden = "Visible";

    [ObservableProperty]
    private bool _isEnabled = false;

    private void Bgw_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        IsDataGridHidden = "Visible";
        IsProgressBarHidden = "Hidden";
        IsEnabled = true;
    }

    private void LoadTables(object? sender, DoWorkEventArgs e)
    {
        DbModel.TableNames = _databaseConnectionService.GetTables();
        DbModel.TableList = _databaseConnectionService.FillTables(DbModel.TableNames);
        DbModel.SelectedTable = DbModel.TableList.ElementAt(DbModel.SelectedIndex);
    }

    [RelayCommand]
    public void RejectChanges()
    {
        DbModel.SelectedTable.RejectChanges();
        ErrorMsg = "";
    }

    [RelayCommand]
    public void AcceptChanges()
    {
            ErrorMsg = "";
            DataTable? x = DbModel.SelectedTable.GetChanges();
            if (x != null)
            {
                try
                {
                    _databaseConnectionService.UpdateTable(DbModel.SelectedIndex, DbModel.SelectedTable);
                    DbModel.SelectedTable.AcceptChanges();
                    ErrorMsg = "Pomyślnie zatwierdzono zmiany!";
                }
                catch (OracleException ex)
                {
                    ErrorMsg = ex.Message;
                }
            }
            else
                ErrorMsg = "Nie wprowadzono zmian";
    }

    [RelayCommand]
    public void AddRow()
    {
        DataRow dr = DbModel.SelectedTable.NewRow();
        DbModel.SelectedTable.Rows.Add(dr);
        ErrorMsg = "";
    }

    [RelayCommand]
    public void Search()
    {
        ErrorMsg = "";
        if (!SearchBox.Equals(string.Empty))
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
