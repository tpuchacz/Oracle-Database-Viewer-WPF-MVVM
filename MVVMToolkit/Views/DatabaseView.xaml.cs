using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MVVMToolkit;

namespace MVVMToolkit.Views;

/// <summary>
/// Logika interakcji dla klasy DatabaseView.xaml
/// </summary>
public partial class DatabaseView : UserControl
{
    public DatabaseView()
    {
        InitializeComponent();
    }

    //Wymagane dla wprowadzania pustych wartości
    private void oracleData_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        ((DataGridBoundColumn)e.Column).Binding.TargetNullValue = string.Empty;
    }
}
