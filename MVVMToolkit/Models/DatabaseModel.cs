using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MVVMToolkit;
using MVVMToolkit.Services;
using Oracle.ManagedDataAccess.Client;
using System.ComponentModel;

namespace MVVMToolkit.Models;

public partial class DatabaseModel : ObservableRecipient
{
    public DatabaseModel()
    {
        TableList = new List<DataTable>();
        TableNames = new List<string>();
        SelectedTable = new DataTable();
    }

    [ObservableProperty]
    private List<DataTable> _tableList;

    [ObservableProperty]
    private int _selectedIndex;

    [ObservableProperty]
    private List<string> _tableNames;

    [ObservableProperty]
    private DataTable _selectedTable;

    partial void OnSelectedIndexChanged(int value)
    {
        SelectedTable = TableList.ElementAt(SelectedIndex);
    }

}
