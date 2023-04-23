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

namespace MVVMToolkit;

public partial class DatabaseViewModel : BaseViewModel, IRecipient<SuccesfulLogin>
{
    private OracleConnection? conn;

    [ObservableProperty]
    private string? connStr;

    [ObservableProperty]
    private OracleCredential? cred;

    [ObservableProperty]
    private string? _errorMsg;

    public DatabaseViewModel()
    {
        IMessenger messenger = Messenger;
        messenger.Register<SuccesfulLogin>(this);
    }

    //Przyjmujemy SuccesfulLogin od LoginViewModel
    public void Receive(SuccesfulLogin message)
    {
        ConnStr = message.ConnectionString;
        Cred = message.Credentials;
    }

    //Kolekcja pracowników
    [ObservableProperty]
    private ObservableCollection<EmployeesModel> _oracleDBData = new ObservableCollection<EmployeesModel>();

    [RelayCommand]
    private void LoadData()
    {
        IMessenger messenger = Messenger;
        messenger.Send("GetConnection");//Poprzez wysłanie wiadomości do LoginViewModel uzyskuje SuccesfulLogin
        if (string.IsNullOrWhiteSpace(ConnStr) || Cred == null) { ErrorMsg = "Pusto"; }
        else
        {
            using (conn = new OracleConnection(ConnStr, Cred))
            {
                conn.Open();
                using (OracleCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM employees ORDER BY employee_id";
                    cmd.CommandType = CommandType.Text;
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        
                        while (reader.Read())
                        {
                            EmployeesModel employee = new EmployeesModel();
                            employee.EmployeeId = reader.GetInt32(0);
                            if (!reader.IsDBNull(1))
                                employee.FirstName = reader.GetString(1);
                            employee.LastName = reader.GetString(2);
                            employee.Email = reader.GetString(3);
                            if (!reader.IsDBNull(4))
                                employee.PhoneNumber = reader.GetString(4);
                            employee.HireDate = reader.GetDateTime(5);
                            employee.JobId = reader.GetString(6);
                            employee.Salary = reader.GetDouble(7);
                            if(!reader.IsDBNull(8))
                                employee.CommissionPct = reader.GetDouble(8);
                            if (!reader.IsDBNull(9))
                                employee.ManagerId = reader.GetInt32(9);
                            if (!reader.IsDBNull(10))
                                employee.DepartmentId = reader.GetInt32(10);
                            OracleDBData.Add(employee);
                        }
                    }
                }
            }
        }
    }
}
