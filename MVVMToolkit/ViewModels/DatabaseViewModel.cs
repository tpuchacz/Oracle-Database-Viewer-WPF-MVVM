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
using System.ComponentModel.DataAnnotations;
using ControlzEx.Standard;
using System.Drawing;
using System.Windows.Controls;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace MVVMToolkit.ViewModels;

public partial class DatabaseViewModel : BaseViewModel
{
    private readonly IDatabaseConnectionService _databaseConnectionService;

    private IDialogCoordinator _dialogCoordinator; //From MahApps

    public DatabaseViewModel(DatabaseModel databaseModel, IDatabaseConnectionService databaseConnectionService, IDialogCoordinator instance)
    {
        DbModel = databaseModel;
        _databaseConnectionService = databaseConnectionService;
        _dialogCoordinator = instance;
        //Loading data into tables in the background
        using (BackgroundWorker bgw = new BackgroundWorker())
        {
            bgw.RunWorkerCompleted += Bgw_RunWorkerCompleted;
            bgw.DoWork += LoadTables;
            bgw.RunWorkerAsync();
        }
    }

    [ObservableProperty]
    private string? _errorMsg = "";

    [ObservableProperty]
    private string? _searchBox = "";

    [ObservableProperty]
    private DatabaseModel _dbModel;

    [ObservableProperty]
    private string _isDataGridHidden = "Hidden";

    [ObservableProperty]
    private string _isProgressBarHidden = "Visible";

    [ObservableProperty]
    private bool _isInteractionEnabled = false; //While data loading block all interaction; binded to buttons etc.

    private void Bgw_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        IsDataGridHidden = "Visible";
        IsProgressBarHidden = "Hidden";
        IsInteractionEnabled = true;
    }

    private void LoadTables(object? sender, DoWorkEventArgs e)
    {
        DbModel.TableNames = _databaseConnectionService.GetTables();
        DbModel.TableList = _databaseConnectionService.FillTables(DbModel.TableNames);
        DbModel.SelectedTable = DbModel.TableList.ElementAt(DbModel.SelectedIndex);
    }

    [RelayCommand]
    public async void AcceptChanges()
    {
        ErrorMsg = "";
        //Checking for changes
        DataTable? changed = DbModel.SelectedTable.GetChanges();
        if (changed != null)
        {
            //Dialog Service from MahApps implemented with dependency injection
            var result = await _dialogCoordinator.ShowMessageAsync(this, "Potwierdź zmiany", "Po kliknięciu OK zmiany zostaną zatwierdzone i wysłane do bazy danych.\n" +
            "Czy chcesz kontynuować?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
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
        }
        else
            ErrorMsg = "Nie wprowadzono zmian";
    }

    [RelayCommand]
    public async void RejectChanges()
    {
        var result = await _dialogCoordinator.ShowMessageAsync(this, "Cofnij zmiany", "Czy napewno chcesz cofnąć zmiany?",
                                                               MessageDialogStyle.AffirmativeAndNegative);
        if (result == MessageDialogResult.Affirmative)
        {
            DbModel.SelectedTable.RejectChanges();
            ErrorMsg = "";
        }
    }

    [RelayCommand]
    public void AddRow()
    {
        DataRow dr = DbModel.SelectedTable.NewRow();
        //Checking PrimaryKey probably unnecessary
        if(DbModel.SelectedTable.PrimaryKey != null)
        {
            if (DbModel.SelectedTable.Columns[0].DataType == typeof(Int16)
                || DbModel.SelectedTable.Columns[0].DataType == typeof(Int32)
                || DbModel.SelectedTable.Columns[0].DataType == typeof(Int64)) //The ID field might be different types of int/other, sticking to int for now
            {
                try
                {
                    //Finding maximum ID value and incrementing
                    int maxID = Convert.ToInt32(DbModel.SelectedTable.Compute($"max([{DbModel.SelectedTable.Columns[0]}])", string.Empty));
                    dr[0] = maxID + 1;
                }
                catch (InvalidCastException ex) { } //Mostly in case there are no IDs
            }
        }
        DbModel.SelectedTable.Rows.Add(dr);
        ErrorMsg = "";
    }

    [RelayCommand]
    public void Search()
    {
        //Filtering results in current table
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
        //Clearing search filter if search field empty
        if (newValue.Equals(string.Empty))
            SearchCommand.Execute(null);
    }
}
