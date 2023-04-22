using CommunityToolkit.Mvvm.ComponentModel;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVMToolkit;

public partial class EmployeesModel : ObservableObject
{
    [ObservableProperty]
    private int _employeeId;

    [ObservableProperty]
    private string? _firstName;

    [ObservableProperty]
    private string _lastName;

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string? _phoneNumber;

    [ObservableProperty]
    private DateTime _hireDate;

    [ObservableProperty]
    private string _jobId;

    [ObservableProperty]
    private int? _salary;

    [ObservableProperty]
    private int? _commissionPct;

    [ObservableProperty]
    private int? _managerId;

    [ObservableProperty]
    private int? _departmentId;
}
