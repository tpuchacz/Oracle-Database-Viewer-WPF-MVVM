using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MVVMToolkit;
using MVVMToolkit.Services;
using Oracle.ManagedDataAccess.Client;

namespace MVVMToolkit.Models;

public partial class DatabaseModel : ObservableRecipient
{

    private List<DataTable> _tableList;
    public List<DataTable> TableList
    {
        get { return _tableList; }
        set { _tableList = value; }
    }
    public DatabaseModel()
    {
        TableList = new List<DataTable>();
    }

    [ObservableProperty]
    private int _selectedIndex = 0;

    [ObservableProperty]
    private List<string> _tableNames;

    [ObservableProperty]
    private DataTable _selectedTable;

    partial void OnSelectedIndexChanged(int value)
    {
        SelectedIndex = value;
        SelectedTable = TableList.ElementAt(SelectedIndex);
    }
}
